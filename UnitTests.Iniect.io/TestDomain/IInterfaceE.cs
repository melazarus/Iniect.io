namespace UnitTests.Iniect.io.TestDomain
{
    public interface IInterfaceE
    {
        IInterfaceD CircularDependencyD { get; set; }
    }
}