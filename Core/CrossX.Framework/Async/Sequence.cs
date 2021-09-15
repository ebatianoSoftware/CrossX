using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrossX.Framework.Async
{
    public sealed class Sequence
    {
        private static readonly Sequence NextFrameSequence = new Sequence { frames = 0 };

        private int frames;
        private double seconds;
        private IEnumerator<Sequence> enumerator;
        private Func<bool> condition;

        private volatile int cancel = 0;

        internal bool IsRun 
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

        public bool IsFinished
        {
            get
            {
                lock (lockObj) return isFinished;
            }

            private set
            {
                lock (lockObj)
                {
                    isFinished = value;
                    finishedEvent?.Set();
                }
            }
        }

        public bool IsCanceled
        {
            get
            {
                lock (lockObj) return isCancelled;
            }

            private set
            {
                lock (lockObj)
                {
                    isCancelled = value;
                    finishedEvent?.Set();
                }
            }
        }

        private Task task;
        private AutoResetEvent finishedEvent;
        private object lockObj = new object();
        private bool isFinished;
        private bool isRun;
        private bool isCancelled;

        public Task Task
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

        private Sequence() { }

        public static Sequence WaitForNextFrame() => NextFrameSequence;
        public static Sequence WaitForFrames(int frames) => new Sequence { frames = frames };
        public static Sequence WaitForSeconds(double seconds) => new Sequence { seconds = seconds };
        public static Sequence WaitForCondition(Func<bool> func) => new Sequence { condition = func };

        public static Sequence DelayAction(double seconds, Action action) => Agregate(Delay(seconds, action));

        private static IEnumerable<Sequence> Delay(double seconds, Action action)
        {
            yield return WaitForSeconds(seconds);
            action.Invoke();
        }

        public static Sequence Agregate(IEnumerable<Sequence> Sequence)
        {
            var enumerator = Sequence.GetEnumerator();
            if (!enumerator.MoveNext()) return WaitForNextFrame();

            return new Sequence { enumerator = enumerator };
        }

        public void Cancel() => cancel++;

        internal bool ShouldRemove(TimeSpan timeSpan)
        {
            if (cancel > 0)
            {
                IsCanceled = true;
                return true;
            }

            if (frames > 0) frames--;
            if (seconds > 0) seconds -= timeSpan.TotalSeconds;

            if (enumerator != null)
            {
                if (enumerator.Current.ShouldRemove(timeSpan))
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

            if (finished) IsFinished = true;
            return finished;
        }
    }
}
