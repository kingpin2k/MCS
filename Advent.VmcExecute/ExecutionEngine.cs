using Advent.Common.Diagnostics;
using Advent.Common.UI;
using Microsoft.DirectX.DirectDraw;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Advent.VmcExecute
{
    internal class ExecutionEngine
    {
        private const int DirectXRetry = 250;
        private const int DirectXTimeout = 10000;
        private readonly ProcessStartInfo psi;
        private readonly ExecutionInfo execInfo;
        private GlobalHook hook;
        private EventHandler executionStarted;
        private EventHandler processStarted;
        private EventHandler processExited;
        private EventHandler executionFinished;
        private EventHandler<Advent.VmcExecute.ErrorEventArgs> executionError;

        public ProcessJob ProcessJob { get; private set; }

        public Process ExecutingProcess { get; set; }

        public bool RequiresKeyboardHook
        {
            get
            {
                if (this.execInfo.CloseKeys != null && this.execInfo.CloseKeys.Count > 0)
                    return true;
                if (this.execInfo.KillKeys != null)
                    return this.execInfo.KillKeys.Count > 0;
                else
                    return false;
            }
        }

        public static string MediaCenterPath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName, "ehome");
            }
        }

        public event EventHandler ExecutionStarted
        {
            add
            {
                EventHandler eventHandler = this.executionStarted;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.executionStarted, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.executionStarted;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.executionStarted, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler ProcessStarted
        {
            add
            {
                EventHandler eventHandler = this.processStarted;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.processStarted, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.processStarted;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.processStarted, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler ProcessExited
        {
            add
            {
                EventHandler eventHandler = this.processExited;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.processExited, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.processExited;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.processExited, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler ExecutionFinished
        {
            add
            {
                EventHandler eventHandler = this.executionFinished;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.executionFinished, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.executionFinished;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.executionFinished, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler<Advent.VmcExecute.ErrorEventArgs> ExecutionError
        {
            add
            {
                EventHandler<Advent.VmcExecute.ErrorEventArgs> eventHandler = this.executionError;
                EventHandler<Advent.VmcExecute.ErrorEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<Advent.VmcExecute.ErrorEventArgs>>(ref this.executionError, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<Advent.VmcExecute.ErrorEventArgs> eventHandler = this.executionError;
                EventHandler<Advent.VmcExecute.ErrorEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<Advent.VmcExecute.ErrorEventArgs>>(ref this.executionError, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        internal ExecutionEngine(ExecutionInfo info, GlobalHook globalHook)
        {
            this.execInfo = info;
            this.hook = globalHook;
            this.psi = new ProcessStartInfo()
            {
                FileName = this.execInfo.FileName,
                Arguments = this.execInfo.Arguments,
                UseShellExecute = true
            };
            if (this.psi.FileName == null)
                throw new ArgumentException("A file to execute must be provided.");
        }

        public void BeginExecute()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.Execute));
        }

        protected virtual void OnExecutionStarted()
        {
            EventHandler eventHandler = this.executionStarted;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        protected virtual void OnProcessStarted()
        {
            EventHandler eventHandler = this.processStarted;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        protected virtual void OnProcessExited()
        {
            EventHandler eventHandler = this.processExited;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        protected virtual void OnExecutionFinished()
        {
            EventHandler eventHandler = this.executionFinished;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        protected virtual void OnExecutionError(Exception ex)
        {
            EventHandler<Advent.VmcExecute.ErrorEventArgs> eventHandler = this.executionError;
            if (eventHandler == null)
                return;
            eventHandler((object)this, new Advent.VmcExecute.ErrorEventArgs(ex));
        }

        private static void WaitForDirectXExclusive()
        {
            ExecutionEngine.WaitForDirectXExclusiveInner();
        }

        private static void WaitForDirectXExclusiveInner()
        {
            using (Form form = new Form())
            {
                form.CreateControl();
                bool flag = false;
                int num = 0;
                while (!flag)
                {
                    if (num <= 10000)
                    {
                        try
                        {
                            using (Device device = new Device())
                            {
                                device.SetCooperativeLevel((Control)form, CooperativeLevelFlags.Normal);
                                flag = device.TestCooperativeLevel();
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("Error checking for DirectX exclusive mode: " + (object)ex);
                        }
                        if (!flag)
                        {
                            if (num == 0)
                                Trace.TraceInformation("Waiting for DirectX exclusive mode...");
                            num += 250;
                            Thread.Sleep(250);
                        }
                    }
                    else
                        break;
                }
                if (flag)
                    Trace.WriteLine("DirectX exclusive mode is available.");
                else
                    Trace.TraceWarning("Timed out waiting for DirectX exclusive mode. Launching anyway...");
            }
        }

        private void Execute(object state)
        {
            this.OnExecutionStarted();
            try
            {
                if (this.execInfo.MinimizeMediaCenter)
                {
                    IntPtr window = Advent.VmcExecute.NativeMethods.FindWindow("eHome Render Window", (string)null);
                    if (window != IntPtr.Zero)
                        Advent.VmcExecute.NativeMethods.ShowWindow(window, Advent.VmcExecute.NativeMethods.WindowShowStyle.Minimize);
                    else
                        Trace.TraceWarning("Could not find ehome window.");
                }
                if (this.execInfo.RequiresDirectX && !ExecutionEngine.Is64BitOs())
                    ExecutionEngine.WaitForDirectXExclusive();
                using (ProcessJob processJob = new ProcessJob())
                {
                    this.ProcessJob = processJob;
                    this.psi.WorkingDirectory = Path.GetDirectoryName(this.psi.FileName);
                    this.psi.UseShellExecute = true;
                    Trace.TraceInformation("Launching \"{0}\" {1}", (object)this.psi.FileName, (object)this.psi.Arguments);
                    this.ExecutingProcess = Process.Start(this.psi);
                    if (this.ExecutingProcess != null)
                    {
                        try
                        {
                            this.ProcessJob.AssignProcess(this.ExecutingProcess);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                        this.OnProcessStarted();
                        if (this.RequiresKeyboardHook)
                            this.hook.KeyDown += new KeyEventHandler(this.HookKeyDown);
                        this.ExecutingProcess.WaitForExit();
                        if (this.RequiresKeyboardHook)
                            this.hook.KeyDown -= new KeyEventHandler(this.HookKeyDown);
                        bool flag = false;
                        while (!flag)
                        {
                            flag = true;
                            foreach (Process process in this.ProcessJob.Processes)
                            {
                                if (!process.HasExited)
                                {
                                    Trace.TraceInformation(string.Format("Main process exited, waiting for process: {0}({1})", (object)process.ProcessName, (object)process.Id));
                                    flag = false;
                                    process.WaitForExit();
                                }
                            }
                        }
                        this.OnProcessExited();
                        if (this.ExecutingProcess.ExitCode != 0)
                            Trace.TraceWarning("Process exited with non-zero error code " + (object)this.ExecutingProcess.ExitCode + ".");
                    }
                    else
                        Trace.TraceError("Could not start process, Process.Start returned null.");
                }
            }
            catch (Exception ex)
            {
                this.OnExecutionError(ex);
            }
            Trace.TraceInformation("Launching Media Center...");
            ExecutionEngine.LaunchMediaCenter(false, false, false);
            this.OnExecutionFinished();
        }

        private void HookKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.execInfo.CloseKeys != null && this.execInfo.CloseKeys.Contains(e.KeyData))
                {
                    this.ProcessJob.CloseMainWindows();
                }
                else
                {
                    if (this.execInfo.KillKeys == null || !this.execInfo.KillKeys.Contains(e.KeyData))
                        return;
                    this.ProcessJob.Kill();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public static Process LaunchMediaCenter(bool forceFullScreen, bool forceWideScreen, bool gdi)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(ExecutionEngine.MediaCenterPath, "ehshell.exe"));
            startInfo.WindowStyle = forceFullScreen ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal;
            string str = string.Empty;
            if (forceWideScreen)
                str = str + " /widescreen";
            if (gdi)
                str = str + " /gdi";
            startInfo.Arguments = str.Trim();
            return Process.Start(startInfo);
        }

        [DllImport("Kernel32.dll")]
        private static extern int GetSystemWow64Directory([MarshalAs(UnmanagedType.LPStr), In, Out] StringBuilder lpBuffer, [MarshalAs(UnmanagedType.U4)] uint size);

        private static bool Is64BitOs()
        {
            StringBuilder lpBuffer = new StringBuilder(256);
            return ExecutionEngine.GetSystemWow64Directory(lpBuffer, (uint)lpBuffer.Length) != 0;
        }
    }
}
