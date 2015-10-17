using Iniect.io;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Iniect.io
{
    public class SuperTestClass
    {
        [TestInitialize]
        public void TestInit()
        {
            //Since the Factory is static we need to call Reset before each test
            //Otherwise the order of the tests my influence their outcome
            Factory.Reset();
        }
    }
}