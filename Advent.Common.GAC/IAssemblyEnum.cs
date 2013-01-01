
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    [Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IAssemblyEnum
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetNextAssembly(IntPtr pvReserved, out IAssemblyName ppName, int flags);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Reset();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Clone(out IAssemblyEnum ppEnum);
    }
}
