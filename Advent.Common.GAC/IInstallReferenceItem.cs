
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("582dac66-e678-449f-aba6-6faaec8a9394")]
    [ComImport]
    internal interface IInstallReferenceItem
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetReference(out IntPtr pRefData, int flags, IntPtr pvReserced);
    }
}
