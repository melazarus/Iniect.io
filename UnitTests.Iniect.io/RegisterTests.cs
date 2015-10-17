using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using UnitTests.Iniect.io.TestDomain;

namespace UnitTests.Iniect.io
{
    [TestClass]
    public class RegisterTests : SuperTestClass
    {
        [TestMethod]
        public void Default_false()
        {
            var result = Factory.IsRegistered<IUnImplemented>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Default_SameAssembly_true()
        {
            var result = Factory.IsRegistered<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_DirectAssembly_true()
        {
            Factory.Register<IInterfaceA>(Assembly.GetExecutingAssembly());
            var result = Factory.IsRegistered<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_AutomaticAssembly_true()
        {
            Factory.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            Factory.Register<IInterfaceA>();
            var result = Factory.IsRegistered<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.NoImplementationFoundException))]
        public void Register_UnImplemented_Exception()
        {
            Factory.Register<IUnImplemented>(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.MultipleImplementationFoundException))]
        public void Register_MultipleImplemented_Exception()
        {
            Factory.Register<IMultipleImplements>(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.NullAssemblyException))]
        public void Register_NullAssembly_exception()
        {
            Assembly nullAssembly = null;
            Factory.Register<IInterfaceA>(nullAssembly);
        }
    }
}