
using System;

namespace Advent.VmcExecute
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ErrorEventArgs(Exception ex)
        {
            this.Exception = ex;
        }
    }
}
