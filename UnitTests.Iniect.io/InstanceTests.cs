using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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