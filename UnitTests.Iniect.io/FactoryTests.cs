using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using UnitTests.Iniect.io.TestDomain;

namespace UnitTests.Iniect.io
{
    [TestClass]
    public class FactoryTests : SuperTestClass
    {
        [TestMethod]
        public void IsRegistered_AutomaticMatchAssembly_true()
        {
            Factory.Static.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Static.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegisterClassIsRegistered_true()
        {
            Factory.Static.Bind<IInterfaceA, ClassA>();
            var result = Factory.Static.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Create_unmapped_IsNull()
        {
            var result = Factory.Static.Create<IUnImplemented>();
        }

        [TestMethod]
        public void Create_mapped_IsNotNull()
        {
            Factory.Static.Bind<IInterfaceA>(Assembly.GetExecutingAssembly());
            var result = Factory.Static.Create<IInterfaceA>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_AutoMapped_IsNotNull()
        {
            Factory.Static.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Static.Create<IInterfaceA>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_AutoMapped_DiNotNull()
        {
            Factory.Static.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Static.Create<IInterfaceA>();
            Assert.IsNotNull(result.DependencyB);
        }

        [TestMethod]
        public void Create_AutoMapped_DiRONull()
        {
            Factory.Static.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Static.Create<IInterfaceA>();
            Assert.IsNull(result.ReadOnlyDependencyB);
        }

        [TestMethod]
        public void Create_ManualMappedAandB_DiNoNull()
        {
            Factory.Static.Bind<IInterfaceA, ClassA>();
            Factory.Static.Bind<IInterfaceB, ClassB>();
            var result = Factory.Static.Create<IInterfaceA>();
            Assert.IsNotNull(result.DependencyB);
        }

        [TestMethod]
        [ExpectedException(typeof(InterfaceNotImplementedByClassException))]
        public void Register_WrongClass_Exception()
        {
            Factory.Static.Bind<IInterfaceA, ClassB>();
        }

        [TestMethod]
        public void CreateTwice_SameObject()
        {
            Factory.Static.Bind<IInterfaceA, ClassA>();
            var result1 = Factory.Static.Create<IInterfaceA>();
            var result2 = Factory.Static.Create<IInterfaceA>();
            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void RegisterInstanceCreate_areEqual()
        {
            var objectA = new ClassA();
            Factory.Static.Bind<IInterfaceA>(objectA);
            var createdObjectA = Factory.Static.Create<IInterfaceA>();
            Assert.AreSame(objectA, createdObjectA);
        }

        [TestMethod]
        public void Create_SelfReferencing_sameObject()
        {
            var x = Factory.Static.Create<ISelfReference>();

            Assert.AreSame(x, x.SelfReference);
        }
    }
}