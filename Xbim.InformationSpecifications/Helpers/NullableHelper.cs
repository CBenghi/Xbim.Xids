using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.CodeAnalysis
{
#if NETSTANDARD2_0
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }
#endif

}
