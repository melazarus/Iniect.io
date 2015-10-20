using System;

namespace Iniect.io
{
    public class TypeIsNotAnInterfaceException : Exception
    {
        public override string Message { get; } = "The type provided is not an Interface";
    }
}