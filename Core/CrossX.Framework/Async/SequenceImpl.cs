using CrossX.Abstractions.Async;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrossX.Framework.Async
{
    public class SequenceImpl: Sequence
    {
        private int frames;
        private double seconds;
        private IEnumerator<Sequence> enumerator;
        private Func<bool> condition;

        private volatile int cancel = 0;

        private Task task;
        private AutoResetEvent finishedEvent;
        private object lockObj = new object();
        private bool isFinished;
        private bool isRun;
        private bool isCancelled;

        public bool IsRun
        {
            get
            {
                lock (lockObj) return isRun;
            }

            set
            {
                lock (lockObj)
                {
                    isRun = value;
                }
            }
        }

        public override bool IsFinished
        {
            get
            {
                lock (lockObj) return isFinished;
            }
        }

        private void SetFinished()
        {
            lock (lockObj)
            {
                isFinished = true;
                finishedEvent?.Set();
            }
        }

        public override bool IsCanceled
        {
            get
            {
                lock (lockObj) return isCancelled;
            }
        }

        public static Sequence Create(int frames, double seconds, Func<bool> condition, IEnumerator<Sequence> enumerator)
        {
            return new SequenceImpl
            {
                frames = frames,
                seconds = seconds,
                condition = condition,
                enumerator = enumerator
            };
        }

        private void SetCanceled()
        {
            lock (lockObj)
            {
                isCancelled = true;
                finishedEvent?.Set();
            }
        }

        public override Task Task
        {
            get
            {
                lock (lockObj)
                {
                    if (isFinished) return Task.CompletedTask;

                    if (IsRun == false) throw new InvalidOperationException("Cannot wait for not run sequences!");

                    if (task == null)
                    {
                        finishedEvent = new AutoResetEvent(false);
                        task = Task.Run(finishedEvent.WaitOne);
                    }
                    return task;
                }
            }
        }

        public override void Cancel() => cancel++;

        public bool ShouldRemove(TimeSpan timeSpan)
        {
            if (cancel > 0)
            {
                SetCanceled();
                return true;
            }

            if (frames > 0) frames--;
            if (seconds > 0) seconds -= timeSpan.TotalSeconds;

            if (enumerator != null)
            {
                if ( ((SequenceImpl)enumerator.Current).ShouldRemove(timeSpan))
                {
                    if (!enumerator.MoveNext())
                    {
                        enumerator = null;
                    }
                }
            }

            if (condition != null)
            {
                if (condition())
                {
                    condition = null;
                }
            }

            var finished = seconds <= 0 && frames <= 0 && enumerator == null && condition == null;

            if (finished) SetFinished();
            return finished;
        }

    }
}
