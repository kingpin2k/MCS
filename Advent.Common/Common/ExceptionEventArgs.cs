
using System;

namespace Advent.Common
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; protected set; }

        public ExceptionEventArgs(Exception ex)
        {
            this.Exception = ex;
        }
    }
}
