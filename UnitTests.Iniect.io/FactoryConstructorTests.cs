using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitTests.Iniect.io.TestDomain;
using UnitTests.Iniect.io.TestDomain.Constructor;

namespace UnitTests.Iniect.io
{
    [TestClass]
    public class FactoryConstructorTests : SuperTestClass
    {
        [TestMethod]
        public void DiViaInstructor_NotNull()
        {
            var result = Factory.Create<ICtorA>();
            Assert.IsNotNull(result.ExtraProperty);
        }
    }
}