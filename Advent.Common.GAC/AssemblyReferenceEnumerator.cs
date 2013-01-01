
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Advent.Common.GAC
{
    internal class AssemblyReferenceEnumerator : IEnumerator<AssemblyReference>, IDisposable, IEnumerator
    {
        private readonly IInstallReferenceEnum refEnum;
        private AssemblyReference currentReference;
        private bool isComplete;
        private string assemblyName;

        public AssemblyReference Current
        {
            get
            {
                return this.currentReference;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return (object)this.Current;
            }
        }

        public AssemblyReferenceEnumerator(string assemblyName)
        {
            this.assemblyName = assemblyName;
            IAssemblyName ppAssemblyNameObj;
            int errorCode = NativeMethods.CreateAssemblyNameObject(out ppAssemblyNameObj, assemblyName, CreateAssemblyNameObjectFlags.CANOF_PARSE_DISPLAY_NAME, IntPtr.Zero);
            if (errorCode >= 0)
                errorCode = NativeMethods.CreateInstallReferenceEnum(out this.refEnum, ppAssemblyNameObj, 0, IntPtr.Zero);
            if (errorCode >= 0)
                return;
            Marshal.ThrowExceptionForHR(errorCode);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject((object)this.refEnum);
        }

        public bool MoveNext()
        {
            if (this.isComplete)
                return false;
            IInstallReferenceItem ppRefItem;
            int installReferenceItem = this.refEnum.GetNextInstallReferenceItem(out ppRefItem, 0, IntPtr.Zero);
            if (installReferenceItem == -2147024637)
            {
                this.isComplete = true;
                return false;
            }
            else
            {
                if (installReferenceItem < 0)
                    Marshal.ThrowExceptionForHR(installReferenceItem);
                InstallReference installRef = new InstallReference(Guid.Empty, string.Empty, string.Empty);
                IntPtr pRefData;
                int reference = ppRefItem.GetReference(out pRefData, 0, IntPtr.Zero);
                if (reference < 0)
                    Marshal.ThrowExceptionForHR(reference);
                Marshal.PtrToStructure(pRefData, (object)installRef);
                this.currentReference = new AssemblyReference(this.assemblyName, installRef);
                return true;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}
