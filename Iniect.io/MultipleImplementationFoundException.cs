using System;

namespace Iniect.io
{
    public class MultipleImplementationFoundException : Exception
    {
        public override string Message { get; } = "There is more than one implementation for this interface";
    }
}