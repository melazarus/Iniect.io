using System;

namespace Iniect.io
{
    public class NoImplementationFoundException : Exception
    {
        public override string Message { get; } = "Could not find an implementation for this interface";
    }
}