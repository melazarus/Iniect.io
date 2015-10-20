using System;

namespace UnitTests.Iniect.io.AbstractTests
{
    public interface ITestInterface { }

    public abstract class AbstractTestClass : ITestInterface { }

    public class TestClass : AbstractTestClass { }
}