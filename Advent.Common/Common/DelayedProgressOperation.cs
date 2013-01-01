using System;
using System.Threading;

namespace Advent.Common
{
    public class DelayedProgressOperation : ProgressEnabledOperation, IDelayedProgressOperation, IProgressEnabledOperation
    {
        private EventHandler<EventArgs> started ;

        public bool IsStarted { get; private set; }

        public event EventHandler<EventArgs> Started
        {
            add
            {
                EventHandler<EventArgs> eventHandler = this.started;
                EventHandler<EventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.started, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<EventArgs> eventHandler = this.started;
                EventHandler<EventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.started, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public DelayedProgressOperation()
        {
        }

        public DelayedProgressOperation(string description)
            : base(description)
        {
        }

        public virtual void Start()
        {
            if (this.IsStarted)
                throw new InvalidOperationException("The operation has already been started.");
            if (this.IsCompleted)
                throw new InvalidOperationException("The operation is already complete.");
            this.IsStarted = true;
            EventHandler<EventArgs> eventHandler = this.started;
            if (eventHandler == null)
                return;
            eventHandler((object)this, EventArgs.Empty);
        }

        public override void OnProgress(string message, int currentIndex)
        {
            this.CheckStarted();
            base.OnProgress(message, currentIndex);
        }

        public override void OnCompleted()
        {
            this.CheckStarted();
            base.OnCompleted();
        }

        public override void OnAbandoned(Exception ex)
        {
            this.CheckStarted();
            base.OnAbandoned(ex);
        }

        private void CheckStarted()
        {
            if (!this.IsStarted)
                throw new InvalidOperationException("The operation has not been started.");
        }
    }
}
