
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    [Flags]
    internal enum AssemblyCacheFlags
    {
        GAC = 2,
    }

    internal enum AssemblyCommitFlags
    {
        Default = 1,
        Force = 2,
    }

    [ComVisible(false)]
    public class AssemblyCache
    {
        private static AssemblyCache instance;

        public static AssemblyCache Global
        {
            get
            {
                if (AssemblyCache.instance == null)
                    AssemblyCache.instance = new AssemblyCache();
                return AssemblyCache.instance;
            }
        }

        public IEnumerable<string> Assemblies
        {
            get
            {
                return (IEnumerable<string>)new AssemblyCacheCollection();
            }
        }

        public AssemblyReferenceDictionary References { get; private set; }

        public AssemblyCache()
        {
            this.References = new AssemblyReferenceDictionary();
        }

        public IEnumerable<string> SearchAssemblies(string assemblyFilter)
        {
            return (IEnumerable<string>)new AssemblyCacheCollection(assemblyFilter);
        }

        public void InstallAssembly(string assemblyPath, FileInfo reference, bool force)
        {
            this.InstallAssembly(assemblyPath, new InstallReference(InstallReferenceGuid.FilePathGuid, reference.FullName, string.Empty), force);
        }

        public void InstallAssembly(string assemblyPath, string reference, bool force)
        {
            this.InstallAssembly(assemblyPath, new InstallReference(InstallReferenceGuid.OpaqueGuid, reference, string.Empty), force);
        }

        public void InstallAssembly(AssemblyReference reference, bool force)
        {
            this.InstallAssembly(reference.AssemblyName, reference.InstallReference, force);
        }

        public void UninstallAssembly(string assemblyName, FileInfo reference)
        {
            int num = (int)this.UninstallAssembly(assemblyName, new InstallReference(InstallReferenceGuid.FilePathGuid, reference.FullName, string.Empty));
        }

        public void UninstallAssembly(string assemblyName, string reference)
        {
            int num = (int)this.UninstallAssembly(assemblyName, new InstallReference(InstallReferenceGuid.OpaqueGuid, reference, string.Empty));
        }

        public void UninstallAssembly(AssemblyReference reference)
        {
            int num = (int)this.UninstallAssembly(reference.AssemblyName, reference.InstallReference);
        }

        internal AssemblyCacheUninstallDisposition UninstallAssembly(string assemblyName, InstallReference reference)
        {
            AssemblyCacheUninstallDisposition disposition = AssemblyCacheUninstallDisposition.Uninstalled;
            if (reference != null && !InstallReferenceGuid.IsValidGuidScheme(reference.GuidScheme))
                throw new ArgumentException("Invalid reference guid.", "reference");
            IAssemblyCache ppAsmCache;
            int errorCode = NativeMethods.CreateAssemblyCache(out ppAsmCache, 0);
            if (errorCode >= 0)
                errorCode = ppAsmCache.UninstallAssembly(0, assemblyName, reference, out disposition);
            if (errorCode < 0)
                Marshal.ThrowExceptionForHR(errorCode);
            if (disposition != AssemblyCacheUninstallDisposition.Uninstalled)
                Trace.TraceWarning(string.Format("Could not uninstall assembly {0}: {1}", (object)assemblyName, (object)((object)disposition).ToString()));
            return disposition;
        }

        internal string QueryAssemblyInfo(string assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentException("Invalid name", "assemblyName");
            AssemblyInfo assemblyInfo = new AssemblyInfo();
            assemblyInfo.cchBuf = 1024;
            assemblyInfo.currentAssemblyPath = new string(char.MinValue, assemblyInfo.cchBuf);
            IAssemblyCache ppAsmCache;
            int errorCode = NativeMethods.CreateAssemblyCache(out ppAsmCache, 0);
            if (errorCode >= 0)
                errorCode = ppAsmCache.QueryAssemblyInfo(0, assemblyName, ref assemblyInfo);
            if (errorCode < 0)
                Marshal.ThrowExceptionForHR(errorCode);
            return assemblyInfo.currentAssemblyPath;
        }

        private void InstallAssembly(string assemblyPath, InstallReference reference, bool force)
        {
            if (reference != null && !InstallReferenceGuid.IsValidGuidScheme(reference.GuidScheme))
                throw new ArgumentException("Invalid reference guid.", "reference");
            AssemblyCommitFlags assemblyCommitFlags = force ? AssemblyCommitFlags.Force : AssemblyCommitFlags.Default;
            IAssemblyCache ppAsmCache;
            int assemblyCache = NativeMethods.CreateAssemblyCache(out ppAsmCache, 0);
            Marshal.ThrowExceptionForHR(assemblyCache);
            if (assemblyCache < 0)
                return;
            Marshal.ThrowExceptionForHR(ppAsmCache.InstallAssembly((int)assemblyCommitFlags, assemblyPath, reference));
        }
    }
}
