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
            var result = Factory.IsBound<IUnImplemented>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Default_SameAssembly_true()
        {
            var result = Factory.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_DirectAssembly_true()
        {
            Factory.Bind<IInterfaceA>(Assembly.GetExecutingAssembly());
            var result = Factory.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_AutomaticAssembly_true()
        {
            Factory.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            Factory.Bind<IInterfaceA>();
            var result = Factory.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.NoImplementationFoundException))]
        public void Register_UnImplemented_Exception()
        {
            Factory.Bind<IUnImplemented>(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.MultipleImplementationFoundException))]
        public void Register_MultipleImplemented_Exception()
        {
            Factory.Bind<IMultipleImplements>(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(Factory.NullAssemblyException))]
        public void Register_NullAssembly_exception()
        {
            Assembly nullAssembly = null;
            Factory.Bind<IInterfaceA>(nullAssembly);
        }
    }
}