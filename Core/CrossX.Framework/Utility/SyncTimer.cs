using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CrossX.Framework.Utility
{
    public class SyncTimer : IDisposable
    {
        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private Task loopTask;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly TimeSpan tick;

        public SyncTimer(TimeSpan tick)
        {
            loopTask = Task.Run(() => Loop(tick, tokenSource.Token), tokenSource.Token);
            this.tick = tick;
        }

        public void Loop(TimeSpan tick, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var nextTime = stopWatch.Elapsed + tick;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (stopWatch.Elapsed >= nextTime)
                {
                    autoResetEvent.Set();
                    nextTime = stopWatch.Elapsed + tick;
                }
                else
                {
                    autoResetEvent.WaitOne(1);
                }
            }
        }

        public void Wait() => autoResetEvent.WaitOne(tick);

        public void Dispose()
        {
            tokenSource.Cancel();
            loopTask.Wait();
        }
    }
}
