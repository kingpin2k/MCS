
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Advent.Common.GAC
{
    public class AssemblyEnumerator : IEnumerator<string>, IDisposable, IEnumerator
    {
        private readonly IAssemblyEnum assemblyEnum;
        private bool isComplete;
        private string currentAssembly;

        public string Current
        {
            get
            {
                return this.currentAssembly;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return (object)this.Current;
            }
        }

        public AssemblyEnumerator()
            : this((string)null)
        {
        }

        public AssemblyEnumerator(string assemblyName)
        {
            IAssemblyName ppAssemblyNameObj = (IAssemblyName)null;
            int errorCode = 0;
            if (assemblyName != null)
                errorCode = NativeMethods.CreateAssemblyNameObject(out ppAssemblyNameObj, assemblyName, CreateAssemblyNameObjectFlags.CANOF_PARSE_DISPLAY_NAME, IntPtr.Zero);
            if (errorCode >= 0)
                errorCode = NativeMethods.CreateAssemblyEnum(out this.assemblyEnum, IntPtr.Zero, ppAssemblyNameObj, AssemblyCacheFlags.GAC, IntPtr.Zero);
            Marshal.ThrowExceptionForHR(errorCode);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject((object)this.assemblyEnum);
        }

        public bool MoveNext()
        {
            if (this.isComplete)
                return false;
            IAssemblyName ppName;
            Marshal.ThrowExceptionForHR(this.assemblyEnum.GetNextAssembly((IntPtr)0, out ppName, 0));
            if (ppName != null)
            {
                this.currentAssembly = AssemblyEnumerator.GetFullName(ppName);
                return true;
            }
            else
            {
                this.isComplete = true;
                return false;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private static string GetFullName(IAssemblyName fusionAsmName)
        {
            StringBuilder pDisplayName = new StringBuilder(1024);
            int pccDisplayName = 1024;
            Marshal.ThrowExceptionForHR(fusionAsmName.GetDisplayName(pDisplayName, ref pccDisplayName, 167));
            return ((object)pDisplayName).ToString();
        }
    }
}
