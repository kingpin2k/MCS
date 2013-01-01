using Advent.Common;
using System;

namespace Advent.VmcStudio.Theme.Model
{
    public class ApplyThemesEventArgs : EventArgs
    {
        public ProgressEnabledOperation ApplyOperation { get; private set; }

        public ApplyThemesEventArgs(ProgressEnabledOperation applyOperation)
        {
            this.ApplyOperation = applyOperation;
        }
    }
}
