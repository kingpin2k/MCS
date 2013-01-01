
using Microsoft.Win32;
using System;

namespace Advent.Common.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegistryValueAttribute : Attribute
    {
        public string Name { get; set; }

        public RegistryValueKind ValueKind { get; set; }

        public RegistryValueAttribute()
        {
            this.ValueKind = RegistryValueKind.Unknown;
        }

        public RegistryValueAttribute(string name)
            : this()
        {
            this.Name = name;
        }
    }
}
