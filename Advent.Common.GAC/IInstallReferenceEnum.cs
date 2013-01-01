
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56b1a988-7c0c-4aa2-8639-c3eb5a90226f")]
    [ComImport]
    internal interface IInstallReferenceEnum
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetNextInstallReferenceItem(out IInstallReferenceItem ppRefItem, int flags, IntPtr pvReserced);
    }
}
