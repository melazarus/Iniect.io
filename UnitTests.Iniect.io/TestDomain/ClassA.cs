using System;

namespace UnitTests.Iniect.io.TestDomain
{
    public class ClassA : IInterfaceA, IDisposable
    {
        public string TestA { get; set; }

        public void Dispose()
        {
        }

        public IInterfaceB DependencyB { get; set; }
        public IInterfaceB ReadOnlyDependencyB { get; }
        public IInterfaceB WriteOnlyDependencyB { set; private get; }
    }
}