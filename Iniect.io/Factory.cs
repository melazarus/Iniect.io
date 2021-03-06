﻿/*
    Iniect.io

    Version: 1.0.0 Beta1
    Author: Hans Polders

    Summary:
        Iniect.io is a simple IoC container.

    Features:
        Zero config:    If a single implementation of an interface is found in the same assembly than the Interface
                        then the factory will use this by default.
                        calling Factory.Create<InterfaceName>(); will suffice

        DefaultAssembly:You can set the default assembly. In that case the factory will first look in that assembly
                        for an implementation before looking in the Interface assembly

        Register types: You can manually bind Interfaces to Classes or Objects.
            - interface from assembly
                go find the implementation in the given assembly
            - interface to class
                hard bind the interface to the class
            - interface to instance
                hard bind the interface to an instance of a class (Object)

        Auto resolve dependencies
            in constructors, properties and public fields

        Reuse objects (default on)
            elliminates circular references.

        Force create new object

        Maximum dependency injection level (default = 5)

        Provide extra parameters to the constructor
            This will allways create a new instance!
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Iniect.io
{
    public class Factory
    {
        #region Properties

        private static Factory _staticFactory;
        public static Factory Static => _staticFactory ?? (_staticFactory = new Factory());

        public Assembly AutomaticMatchAssembly { get; set; } = null;

        public int MaxDiLevel { get; set; } = 5;

        #endregion Properties

        #region fields

        private ConcurrentDictionary<Type, Type> _matchRegistry;
        private ConcurrentDictionary<Type, WeakReference> _instanceRegistry;

        private ConcurrentDictionary<Type, Type> MatchRegistry => _matchRegistry ?? (_matchRegistry = new ConcurrentDictionary<Type, Type>());

        #endregion fields

        public Factory()
        {
            _instanceRegistry = new ConcurrentDictionary<Type, WeakReference>();
        }

        // Public methods call their private-non-generic counterparts.

        #region Public Methods

        /// <summary>
        /// Binds interfact T with an implementation from assembly
        /// </summary>
        /// <typeparam name="T">Interface to Bind</typeparam>
        /// <param name="assembly"></param>
        public void Bind<T>(Assembly assembly = null)
        {
            var type = typeof(T);

            assembly = assembly ?? AutomaticMatchAssembly;

            if (assembly == null) throw new NullAssemblyException("Assembly cannot be null");

            Bind(type, assembly);
        }

        public void Bind<TSource, TTarget>()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            Bind(sourceType, targetType);
        }

        public void Bind<TInterface>(TInterface implementation)
        {
            var interfaceType = typeof(TInterface);
            _instanceRegistry.TryAdd(interfaceType, new WeakReference(implementation));
        }

        public bool IsBound<T>()
        {
            return IsBound(typeof(T));
        }

        /// <summary>
        /// Takes the current object and injects dependencies into properties/fields
        /// </summary>
        /// <param name="o"></param>
        public void Inject(object o)
        {
            InjectProperties(o);
        }

        public T Create<T>()
        {
            var ttype = typeof(T);

            return (T)Create(ttype);
        }

        /// <summary>
        /// Create a new Instance of type T
        /// Dependencies will be stored/retrieved from the InstanceRegistry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateNew<T>() { throw new NotImplementedException(); }

        /// <summary>
        /// Resets to Facotry defaults
        /// </summary>
        public void Reset()
        {
            MatchRegistry.Clear();
            _instanceRegistry.Clear();
            AutomaticMatchAssembly = null;
            MaxDiLevel = 5;
        }

        #endregion Public Methods

        #region Private Methods

        private void Bind(Type type, Assembly assembly)
        {
            var implementations = assembly
                .GetTypes()
                .Where(type.IsAssignableFrom)
                .Where(x => !x.IsInterface)
                .ToList();
            if (implementations.Count == 0) throw new NoImplementationFoundException("Unable to find a implementation");
            if (implementations.Count > 1) throw new MultipleImplementationFoundException("Found more than one implementation");

            Bind(type, implementations.First());
        }

        private bool TryBind(Type type, Assembly assembly)
        {
            try
            {
                Bind(type, assembly);
                return true;
            }
            catch (Exception ex)
                when (ex is NoImplementationFoundException || ex is MultipleImplementationFoundException)
            {
                return false;
            }
        }

        private void Bind(Type sourceType, Type targetType)
        {
            if (targetType.IsAbstract) throw new TypeBindException("Target type cannot be abstract");
            if (targetType.IsInterface) throw new TypeBindException("Target type cannot be an interface");
            if (!(sourceType.IsInterface || sourceType.IsAbstract)) throw new TypeBindException("Source type must be Interface or Abstract");

            if (!sourceType.IsAssignableFrom(targetType)) throw new InterfaceNotImplementedByClassException("Target type is not assignalble to source type");
            if (MatchRegistry.ContainsKey(sourceType)) return;

            MatchRegistry.TryAdd(sourceType, targetType);
        }

        private bool IsBound(Type interfaceType)
        {
            TryBind(interfaceType, AutomaticMatchAssembly ?? interfaceType.Assembly);

            return MatchRegistry.ContainsKey(interfaceType);
        }

        private object CreateInstance(Type classType)
        {
            // check against interface and abstract
            if (!IsInstantiatable(classType)) throw new InvalidTypeException("Type must be instantiatable (no Interface or Abstract Class)");

            // find largest matching ctor
            var constructor = FindLargestMatchingConstructor(classType);
            if (constructor == null) throw new Exception("No usefull constructor found");
            var parameters = SetParameters(constructor);
            return constructor.Invoke(parameters);
        }

        private object[] SetParameters(ConstructorInfo constructor)
        {
            return constructor.GetParameters().Select(parameterInfo => Create(parameterInfo.ParameterType)).ToArray();
        }

        private ConstructorInfo FindLargestMatchingConstructor(Type classType)
        {
            var ctors = classType.GetConstructors();

            return ctors.Where(AllParametersOk).OrderByDescending(x => x.GetParameters().Count()).FirstOrDefault();
        }

        private static bool IsInstantiatable(Type classType)
        {
            return (!classType.IsInterface && !classType.IsAbstract);
        }

        private object CreateInstanceFromInterface(Type ttype)
        {
            if (!IsBound(ttype)) throw new Exception("could not find interface-class map");
            {
                var implementationType = MatchRegistry[ttype];

                var instance = CreateInstance(implementationType);

                if (!_instanceRegistry.ContainsKey(ttype))
                {
                    _instanceRegistry.TryAdd(ttype, new WeakReference(instance));
                }

                InjectProperties(instance);
                InjectPublicFields(instance);

                return instance;
            }
        }

        private void InjectPublicFields(object instance)
        {
            var ttype = instance.GetType();

            var diFields = ttype.GetFields()
                .Where(x => x.FieldType.IsInterface && IsBound(x.FieldType) && x.GetValue(instance) == null)
                .ToList();
            foreach (var fieldInfo in diFields)
            {
                fieldInfo.SetValue(instance, Create(fieldInfo.FieldType));
            }
        }

        private bool AllParametersOk(ConstructorInfo constructorInfo)
        {
            var ok = true;
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                if (!IsBound(parameterInfo.ParameterType))
                {
                    ok = false;
                }
            }
            return ok;
        }

        private void InjectProperties(object instance)
        {
            var ttype = instance.GetType();

            var diProperties = ttype.GetProperties()
                .Where(x => x.CanWrite && x.PropertyType.IsInterface && IsBound(x.PropertyType) && x.GetValue(instance) == null)
                .ToList();
            foreach (var propertyInfo in diProperties)
            {
                propertyInfo.SetValue(instance, Create(propertyInfo.PropertyType));
            }
        }

        private object Create(Type objectType)
        {
            WeakReference reference;
            if (_instanceRegistry.ContainsKey(objectType) && !_instanceRegistry[objectType].IsAlive) _instanceRegistry.TryRemove(objectType, out reference);
            if (!_instanceRegistry.ContainsKey(objectType)) CreateInstanceFromInterface(objectType);

            return _instanceRegistry[objectType].Target;
        }

        #endregion Private Methods
    }
}