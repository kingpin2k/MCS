using System;

namespace Advent.Common
{
    public class ProgressEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public int CurrentIndex { get; private set; }

        public ProgressEventArgs(string message, int currentIndex)
        {
            this.Message = message;
            this.CurrentIndex = currentIndex;
        }
    }
}
