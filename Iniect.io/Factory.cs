/*
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
            - TODO: Import/Export binding information

        Auto resolve dependencies
            in constructors, properties and TODO: public fields

        Reuse objects (default on)
            elliminates circular references. TODO:  Can be overridden

        Force create new object
            TODO: always create a new object, don't reuse existing ones

        Maximum dependency injection level (default = 5)
            TODO: feature to elumminate endless loops.

        Provide extra parameters to the constructor
            TODO: Factory.Create<IInterface>(new {paramA = "valueA", paramB = 15});
            This will allways create a new instance!
*/

//todo: find a way to create instances of the factory

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

        //Todo: find better name
        public static Assembly AutomaticMatchAssembly { get; set; } = null;

        //TODO: implement
        public static int MaxDiLevel { get; set; } = 5;

        #endregion Properties

        #region fields

        private static ConcurrentDictionary<Type, Type> _matchRegistry;
        private static ConcurrentDictionary<Type, object> _instanceRegistry;

        private static ConcurrentDictionary<Type, Type> MatchRegistry => _matchRegistry ?? (_matchRegistry = new ConcurrentDictionary<Type, Type>());
        private static ConcurrentDictionary<Type, object> InstanceRegistry => _instanceRegistry ?? (_instanceRegistry = new ConcurrentDictionary<Type, object>());

        #endregion fields

        // Public methods call their private-non-generic counterparts.

        #region Public Methods

        /// <summary>
        /// Binds interfact T with an implementation from assembly
        /// </summary>
        /// <typeparam name="T">Interface to Bind</typeparam>
        /// <param name="assembly"></param>
        public static void Bind<T>(Assembly assembly = null)
        {
            var type = typeof(T);

            assembly = assembly ?? AutomaticMatchAssembly;

            if (assembly == null) throw new NullAssemblyException();

            Bind(type, assembly);
        }

        public static void Bind<TInterface, TClass>()
        {
            var interfaceType = typeof(TInterface);
            var classType = typeof(TClass);

            Bind(interfaceType, classType);
        }

        public static void Bind<TInterface>(TInterface implementation)
        {
            var interfaceType = typeof(TInterface);
            InstanceRegistry.TryAdd(interfaceType, implementation);
        }

        public static bool IsBound<T>()
        {
            return IsBound(typeof(T));
        }

        /// <summary>
        /// Takes the current object and injects dependencies into properties/fields
        /// </summary>
        /// <param name="o"></param>
        public static void Inject(object o)
        {
            InjectProperties(o);
        }

        public static T Create<T>()
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
        public static T CreateNew<T>() { throw new NotImplementedException(); }

        /// <summary>
        /// Resets to Facotry defaults
        /// </summary>
        public static void Reset()
        {
            MatchRegistry.Clear();
            InstanceRegistry.Clear();
            AutomaticMatchAssembly = null;
            MaxDiLevel = 5;
        }

        #endregion Public Methods

        #region Private Methods

        private static void Bind(Type type, Assembly assembly)
        {
            var implementations = assembly
                .GetTypes()
                .Where(type.IsAssignableFrom)
                .Where(x => !x.IsInterface)
                .ToList();
            if (implementations.Count == 0) throw new NoImplementationFoundException();
            if (implementations.Count > 1) throw new MultipleImplementationFoundException();

            Bind(type, implementations.First());
        }

        private static bool TryBind(Type type, Assembly assembly)
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

        private static void Bind(Type interfaceType, Type classType)
        {
            if (!interfaceType.IsInterface) throw new Exception("Type must be an interface."); //TODO: replace by custom exception
            if (classType.IsInterface) throw new Exception("classType must be a class."); //TODO: replace by custom exception
            if (!interfaceType.IsAssignableFrom(classType)) throw new InterfaceNotImplementedByClassException();
            if (MatchRegistry.ContainsKey(interfaceType)) return;

            MatchRegistry.TryAdd(interfaceType, classType);
        }

        private static bool IsBound(Type interfaceType)
        {
            TryBind(interfaceType, AutomaticMatchAssembly ?? interfaceType.Assembly);

            return MatchRegistry.ContainsKey(interfaceType);
        }

        //todo: refactor
        private static object CreateInstanceFromInterface(Type ttype)
        {
            if (!IsBound(ttype)) throw new Exception("could not find interface-class map");//TODO: replace by custom exception
            {
                var implementationType = MatchRegistry[ttype];

                var ctors = implementationType.GetConstructors();

                var ctor = ctors.Where(AllParametersOk).OrderByDescending(x => x.GetParameters().Count()).FirstOrDefault();

                if (ctor == null) throw new Exception("No usefull constructor found");

                var parameters = new List<object>();

                foreach (var parameterInfo in ctor.GetParameters())
                {
                    parameters.Add(Create(parameterInfo.ParameterType));
                }

                var instance = ctor.Invoke(parameters.ToArray());

                if (!InstanceRegistry.ContainsKey(ttype))
                {
                    InstanceRegistry.TryAdd(ttype, instance);
                }

                InjectProperties(instance);
                InjectPublicFields(instance);

                return instance;
            }
        }

        //todo: write tests for this.
        private static void InjectPublicFields(object instance)
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

        private static bool AllParametersOk(ConstructorInfo constructorInfo)
        {
            var OK = true;
            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                if (!IsBound(parameterInfo.ParameterType))
                {
                    OK = false;
                }
            }
            return OK;
        }

        private static void InjectProperties(object instance)
        {
            var ttype = instance.GetType();

            var diProperties = ttype.GetProperties() //x.GetValue may throw an exception when the getter throw.
                .Where(x => x.CanWrite && x.PropertyType.IsInterface && IsBound(x.PropertyType) && x.GetValue(instance) == null)
                .ToList();
            foreach (var propertyInfo in diProperties)
            {
                propertyInfo.SetValue(instance, Create(propertyInfo.PropertyType));
            }
        }

        private static object Create(Type objectType)
        {
            if (!objectType.IsInterface) throw new TypeIsNotAnInterfaceException();

            if (!InstanceRegistry.ContainsKey(objectType)) CreateInstanceFromInterface(objectType);

            return InstanceRegistry[objectType];
        }

        #endregion Private Methods

        #region Exceptions

        public class NullAssemblyException : Exception
        {
            public override string Message { get; } = "Assembly cannot be NULL";
        }

        public class NoImplementationFoundException : Exception
        {
            public override string Message { get; } = "Could not find an implementation for this interface";
        }

        public class MultipleImplementationFoundException : Exception
        {
            public override string Message { get; } = "There is more than one implementation for this interface";
        }

        public class InterfaceNotImplementedByClassException : Exception
        {
            public override string Message { get; } = "Class does not implement Interface";
        }

        public class TypeIsNotAnInterfaceException : Exception
        {
            public override string Message { get; } = "The type provided is not an Interface";
        }

        #endregion Exceptions
    }
}