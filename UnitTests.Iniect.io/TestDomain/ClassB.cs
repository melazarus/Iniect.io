using System;

namespace UnitTests.Iniect.io.TestDomain
{
    public class ClassB : IInterfaceB
    {
        public string TestB { get; }
        public DateTime TimeCreated { get; }

        public ClassB()
        {
            TimeCreated = DateTime.Now;
            TestB = "Hello from classB";
        }
    }
}