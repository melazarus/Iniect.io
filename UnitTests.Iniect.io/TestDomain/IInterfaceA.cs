namespace UnitTests.Iniect.io.TestDomain
{
    public interface IInterfaceA
    {
        IInterfaceB DependencyB { get; set; }

        IInterfaceB ReadOnlyDependencyB { get; }

        IInterfaceB WriteOnlyDependencyB { set; }
    }
}