using System;
using System.Linq.Expressions;

namespace Iniect.io
{
    public class InterfaceNotImplementedByClassException : Exception
    {
        public override string Message { get; } = "Class does not implement Interface";
    }
}