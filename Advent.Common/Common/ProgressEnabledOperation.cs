using System;
using System.Threading;

namespace Advent.Common
{
    public class ProgressEnabledOperation : IProgressEnabledOperation
    {
        private readonly ManualResetEvent completedEvent;
        private EventHandler<ProgressEventArgs> progress;
        private EventHandler<EventArgs> completed;
        private EventHandler<ExceptionEventArgs> abandoned;

        public string Description { get; set; }

        public bool IsCompleted { get; private set; }

        public virtual int Count { get; set; }

        public event EventHandler<ProgressEventArgs> Progress
        {
            add
            {
                EventHandler<ProgressEventArgs> eventHandler = this.progress;
                EventHandler<ProgressEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<ProgressEventArgs>>(ref this.progress, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<ProgressEventArgs> eventHandler = this.progress;
                EventHandler<ProgressEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<ProgressEventArgs>>(ref this.progress, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler<EventArgs> Completed
        {
            add
            {
                EventHandler<EventArgs> eventHandler = this.completed;
                EventHandler<EventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.completed, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<EventArgs> eventHandler = this.completed;
                EventHandler<EventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.completed, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler<ExceptionEventArgs> Abandoned
        {
            add
            {
                EventHandler<ExceptionEventArgs> eventHandler = this.abandoned;
                EventHandler<ExceptionEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<ExceptionEventArgs>>(ref this.abandoned, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<ExceptionEventArgs> eventHandler = this.abandoned;
                EventHandler<ExceptionEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<ExceptionEventArgs>>(ref this.abandoned, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public ProgressEnabledOperation()
        {
            this.completedEvent = new ManualResetEvent(false);
        }

        public ProgressEnabledOperation(string description)
            : this()
        {
            this.Description = description;
        }

        public void WaitForCompletion()
        {
            this.completedEvent.WaitOne();
        }

        public virtual void OnProgress(string message, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= this.Count)
                throw new ArgumentException();
            if (this.IsCompleted)
                throw new InvalidOperationException("The operation has already completed.");
            EventHandler<ProgressEventArgs> eventHandler = this.progress;
            if (eventHandler == null)
                return;
            eventHandler((object)this, new ProgressEventArgs(message, currentIndex));
        }

        public virtual void OnCompleted()
        {
            this.IsCompleted = true;
            EventHandler<EventArgs> eventHandler = this.completed;
            if (eventHandler != null)
                eventHandler((object)this, EventArgs.Empty);
            this.completedEvent.Set();
        }

        public virtual void OnAbandoned(Exception ex)
        {
            this.IsCompleted = true;
            EventHandler<ExceptionEventArgs> eventHandler = this.abandoned;
            if (eventHandler != null)
                eventHandler((object)this, new ExceptionEventArgs(ex));
            this.completedEvent.Set();
        }
    }
}
