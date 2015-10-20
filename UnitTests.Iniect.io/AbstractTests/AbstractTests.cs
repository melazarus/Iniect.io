using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Iniect.io.AbstractTests
{
    [TestClass]
    public class AbstractTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeBindException))]
        public void BindInterfaceToAbstractClass_Exception()
        {
            var f = new Factory();

            f.Bind<ITestInterface, AbstractTestClass>();
        }

        [TestMethod]
        public void BindInterfaceToClass()
        {
            var f = new Factory();

            f.Bind<ITestInterface, TestClass>();

            Assert.IsNotNull(f.CreateNew<ITestInterface>());
        }

        [TestMethod]
        public void BindAbstractClassToClass()
        {
            var f = new Factory();

            f.Bind<AbstractTestClass, TestClass>();

            Assert.IsNotNull(f.CreateNew<AbstractTestClass>());
        }
    }
}