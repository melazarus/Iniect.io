using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Iniect.io.TestDomain.Constructor;

namespace UnitTests.Iniect.io
{
    [TestClass]
    public class FactoryConstructorTests : SuperTestClass
    {
        [TestMethod]
        public void DiViaInstructor_NotNull()
        {
            var result = Factory.Static.Create<ICtorA>();
            Assert.IsNotNull(result.ExtraProperty);
        }
    }
}