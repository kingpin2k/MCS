
using System;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    [StructLayout(LayoutKind.Sequential)]
    public class InstallReference
    {
        private int cbSize;
        private int flags;
        private Guid guidScheme;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string identifier;
        [MarshalAs(UnmanagedType.LPWStr)]
        private string description;

        public Guid GuidScheme
        {
            get
            {
                return this.guidScheme;
            }
        }

        public string Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public InstallReference(Guid guid, string id, string data)
        {
            this.cbSize = 2 * IntPtr.Size + 16 + (id.Length + data.Length) * 2;
            this.flags = 0;
            int num = this.flags;
            this.guidScheme = guid;
            this.identifier = id;
            this.description = data;
        }
    }
}
