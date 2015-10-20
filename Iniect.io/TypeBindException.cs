using System;

namespace Iniect.io
{
    public class TypeBindException : Exception
    {
        public TypeBindException(string message) : base(message)
        {
        }
    }
}