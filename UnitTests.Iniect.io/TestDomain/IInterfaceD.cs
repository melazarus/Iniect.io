namespace UnitTests.Iniect.io.TestDomain
{
    public interface IInterfaceD
    {
        IInterfaceE CircularDependencyE { get; set; }
    }
}