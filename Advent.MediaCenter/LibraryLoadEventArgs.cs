


using Advent.Common.Interop;
using System;

namespace Advent.MediaCenter
{
    public class LibraryLoadEventArgs : EventArgs
    {
        public string FileName { get; private set; }

        public UnmanagedLibraryAccess Mode { get; set; }

        public LibraryLoadEventArgs(string dllName)
        {
            this.FileName = dllName;
        }
    }
}
