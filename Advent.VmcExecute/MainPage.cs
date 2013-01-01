
namespace Advent.VmcExecute
{
    public class MainPage : VmcExecutePage
    {
        public void Close()
        {
            this.ExecutionEngine.ProcessJob.CloseMainWindows();
        }

        public void Kill()
        {
            this.ExecutionEngine.ProcessJob.Kill();
        }

        public void Activate()
        {
            NativeMethods.SetForegroundWindow(this.ExecutionEngine.ExecutingProcess.MainWindowHandle);
        }
    }
}
