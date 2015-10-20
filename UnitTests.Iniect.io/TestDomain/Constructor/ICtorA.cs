using System.Collections;

namespace UnitTests.Iniect.io.TestDomain.Constructor
{
    public interface ICtorA
    {
        IExtraProperty ExtraProperty { get; }
    }

    internal class CtorA : ICtorA
    {
        public CtorA(IExtraProperty extraProperty)
        {
            ExtraProperty = extraProperty;
        }

        public CtorA(IExtraProperty extraProperty, IEnumerable unrelated)
        {
            ExtraProperty = extraProperty;
        }

        public IExtraProperty ExtraProperty { get; }
    }
}