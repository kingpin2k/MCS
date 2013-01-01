
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{

    internal enum AssemblyCacheUninstallDisposition
    {
        Unknown,
        Uninstalled,
        StillInUse,
        AlreadyUninstalled,
        DeletePending,
        HasInstallReference,
        ReferenceNotFound,
    }

    internal struct AssemblyInfo
    {
        public int cbAssemblyInfo;
        public int assemblyFlags;
        public long assemblySizeInKB;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string currentAssemblyPath;
        public int cchBuf;
    }

    [Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IAssemblyCache
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int UninstallAssembly(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, InstallReference refData, out AssemblyCacheUninstallDisposition disposition);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Reserved(int flags, IntPtr pvReserved, out object ppAsmItem, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int Reserved(out object ppAsmScavenger);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int InstallAssembly(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyFilePath, InstallReference refData);
    }
}
