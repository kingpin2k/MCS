
using System;

namespace Advent.Common.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegistryKeyNameAttribute : Attribute
    {
    }
}
