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
            var result = Factory.Static.IsBound<IUnImplemented>();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Default_SameAssembly_true()
        {
            var result = Factory.Static.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_DirectAssembly_true()
        {
            Factory.Static.Bind<IInterfaceA>(Assembly.GetExecutingAssembly());
            var result = Factory.Static.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Register_AutomaticAssembly_true()
        {
            Factory.Static.AutomaticMatchAssembly = Assembly.GetExecutingAssembly();
            Factory.Static.Bind<IInterfaceA>();
            var result = Factory.Static.IsBound<IInterfaceA>();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(NoImplementationFoundException))]
        public void Register_UnImplemented_Exception()
        {
            Factory.Static.Bind<IUnImplemented>(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleImplementationFoundException))]
        public void Register_MultipleImplemented_Exception()
        {
            Factory.Static.Bind<IMultipleImplements>(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        [ExpectedException(typeof(NullAssemblyException))]
        public void Register_NullAssembly_exception()
        {
            Assembly nullAssembly = null;
            Factory.Static.Bind<IInterfaceA>(nullAssembly);
        }
    }
}