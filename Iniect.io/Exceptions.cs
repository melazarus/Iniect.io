using System;

namespace Iniect.io
{
    public class InterfaceNotImplementedByClassException : Exception
    {
        public InterfaceNotImplementedByClassException(string message) : base(message)
        {
        }
    }

    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message)
        {
        }
    }

    public class NoImplementationFoundException : Exception
    {
        public NoImplementationFoundException(string message) : base(message)
        {
        }
    }

    public class MultipleImplementationFoundException : Exception
    {
        public MultipleImplementationFoundException(string message) : base(message)
        {
        }
    }

    public class NullAssemblyException : Exception
    {
        public NullAssemblyException(string message) : base(message)
        {
        }
    }

    public class TypeBindException : Exception
    {
        public TypeBindException(string message) : base(message)
        {
        }
    }

    public class TypeIsNotAnInterfaceException : Exception
    {
        public TypeIsNotAnInterfaceException(string message) : base(message)
        {
        }
    }
}