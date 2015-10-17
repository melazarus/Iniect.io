namespace UnitTests.Iniect.io.TestDomain
{
    public interface ISelfReference
    {
        ISelfReference SelfReference { get; set; }
    }

    internal class SelfReferenceClass : ISelfReference
    {
        public ISelfReference SelfReference { get; set; }
    }
}