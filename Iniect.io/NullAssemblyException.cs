using System;

namespace Iniect.io
{
    public class NullAssemblyException : Exception
    {
        public override string Message { get; } = "Assembly cannot be NULL";
    }
}