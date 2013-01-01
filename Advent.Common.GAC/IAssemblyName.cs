
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Advent.Common.GAC
{
    [Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IAssemblyName
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetProperty(int PropertyId, IntPtr pvProperty, int cbProperty);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetProperty(int PropertyId, IntPtr pvProperty, ref int pcbProperty);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Finalize();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetDisplayName(StringBuilder pDisplayName, ref int pccDisplayName, int displayFlags);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Reserved(ref Guid guid, object obj1, object obj2, string string1, long llFlags, IntPtr pvReserved, int cbReserved, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetName(ref int pccBuffer, StringBuilder pwzName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetVersion(out int versionHi, out int versionLow);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int IsEqual(IAssemblyName pAsmName, int cmpFlags);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Clone(out IAssemblyName pAsmName);
    }
}
