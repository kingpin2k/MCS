
using Advent.Common.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Advent.Common.Diagnostics
{
    public class ProcessJob : IDisposable
    {
        private IntPtr handle;

        public IEnumerable<Process> Processes
        {
            get
            {
                JOBOBJECT_BASIC_PROCESS_ID_LIST lpJobObjectInfo = new JOBOBJECT_BASIC_PROCESS_ID_LIST();
                if (!Advent.Common.Interop.NativeMethods.QueryInformationJobObject(this.handle, JobObjectInfoClass.JobObjectBasicProcessIdList, ref lpJobObjectInfo, Marshal.SizeOf((object)lpJobObjectInfo), IntPtr.Zero))
                    throw new Win32Exception();
                List<Process> list = new List<Process>();
                for (int index = 0; index < lpJobObjectInfo.NumberOfAssignedProcesses; ++index)
                {
                    Process processById = Process.GetProcessById(lpJobObjectInfo.ProcessIdList[index]);
                    if (processById != null)
                        list.Add(processById);
                }
                return (IEnumerable<Process>)list;
            }
        }

        public ProcessJob()
        {
            SECURITY_ATTRIBUTES lpJobAttributes = new SECURITY_ATTRIBUTES();
            this.handle = Advent.Common.Interop.NativeMethods.CreateJobObject(ref lpJobAttributes, (string)null);
            if (this.handle == IntPtr.Zero)
                throw new Win32Exception();
        }

        ~ProcessJob()
        {
            this.Dispose();
        }

        public void CloseMainWindows()
        {
            foreach (Process process in this.Processes)
                process.CloseMainWindow();
        }

        public void Kill()
        {
            foreach (Process process in this.Processes)
                process.Kill();
        }

        public void AssignProcess(Process process)
        {
            if (process.Handle == IntPtr.Zero)
                throw new ArgumentException("Process does not have a valid handle.");
            if (!Advent.Common.Interop.NativeMethods.AssignProcessToJobObject(this.handle, process.Handle))
                throw new Win32Exception();
        }

        public void Dispose()
        {
            if (!(this.handle != IntPtr.Zero))
                return;
            Advent.Common.Interop.NativeMethods.CloseHandle(this.handle);
            this.handle = IntPtr.Zero;
        }
    }
}
