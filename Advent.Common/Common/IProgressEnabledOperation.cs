using System;

namespace Advent.Common
{
    public interface IProgressEnabledOperation
    {
        bool IsCompleted { get; }

        int Count { get; }

        string Description { get; }

        event EventHandler<ProgressEventArgs> Progress;

        event EventHandler<EventArgs> Completed;

        event EventHandler<ExceptionEventArgs> Abandoned;

        void WaitForCompletion();
    }
}
