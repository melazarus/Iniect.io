using System;

namespace Iniect.io
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message)
        {
        }
    }
}