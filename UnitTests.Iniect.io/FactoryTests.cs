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
            Factory.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegisterClassIsRegistered_true()
        {
            Factory.Bind<IInterfaceA, ClassA>();
            var result = Factory.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Create_unmapped_IsNull()
        {
            var result = Factory.Create<IUnImplemented>();
        }

        [TestMethod]
        public void Create_mapped_IsNotNull()
        {
            Factory.Bind<IInterfaceA>(Assembly.GetExecutingAssembly());
            var result = Factory.Create<IInterfaceA>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_AutoMapped_IsNotNull()
        {
            Factory.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Create<IInterfaceA>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create_AutoMapped_DiNotNull()
        {
            Factory.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Create<IInterfaceA>();
            Assert.IsNotNull(result.DependencyB);
        }

        [TestMethod]
        public void Create_AutoMapped_DiRONull()
        {
            Factory.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            var result = Factory.Create<IInterfaceA>();
            Assert.IsNull(result.ReadOnlyDependencyB);
        }

        [TestMethod]
        public void Create_ManualMappedAandB_DiNoNull()
        {
            Factory.Bind<IInterfaceA, ClassA>();
            Factory.Bind<IInterfaceB, ClassB>();
            var result = Factory.Create<IInterfaceA>();
            Assert.IsNotNull(result.DependencyB);
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.InterfaceNotImplementedByClassException))]
        public void Register_WrongClass_Exception()
        {
            Factory.Bind<IInterfaceA, ClassB>();
        }

        [TestMethod]
        public void CreateTwice_SameObject()
        {
            Factory.Bind<IInterfaceA, ClassA>();
            var result1 = Factory.Create<IInterfaceA>();
            var result2 = Factory.Create<IInterfaceA>();
            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void RegisterInstanceCreate_areEqual()
        {
            var objectA = new ClassA();
            Factory.Bind<IInterfaceA>(objectA);
            var createdObjectA = Factory.Create<IInterfaceA>();
            Assert.AreSame(objectA, createdObjectA);
        }

        [TestMethod]
        public void Create_SelfReferencing_sameObject()
        {
            var x = Factory.Create<ISelfReference>();

            Assert.AreSame(x, x.SelfReference);
        }
    }
}