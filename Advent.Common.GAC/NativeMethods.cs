
using System;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    internal enum CreateAssemblyNameObjectFlags
    {
        CANOF_DEFAULT,
        CANOF_PARSE_DISPLAY_NAME,
    }

    internal class NativeMethods
    {
        [DllImport("fusion.dll")]
        internal static extern int CreateAssemblyEnum(out IAssemblyEnum ppEnum, IntPtr pUnkReserved, IAssemblyName pName, AssemblyCacheFlags flags, IntPtr pvReserved);

        [DllImport("fusion.dll")]
        internal static extern int CreateAssemblyNameObject(out IAssemblyName ppAssemblyNameObj, [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName, CreateAssemblyNameObjectFlags flags, IntPtr pvReserved);

        [DllImport("fusion.dll")]
        internal static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

        [DllImport("fusion.dll")]
        internal static extern int CreateInstallReferenceEnum(out IInstallReferenceEnum ppRefEnum, IAssemblyName pName, int dwFlags, IntPtr pvReserved);
    }
}
