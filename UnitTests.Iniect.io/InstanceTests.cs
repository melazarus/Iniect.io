using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Iniect.io
{
    [TestClass]
    public class InstanceTests : SuperTestClass
    {
        [TestMethod]
        public void CreateInstance()
        {
            var factory = new Factory();
        }
    }
}