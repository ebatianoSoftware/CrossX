using CrossX.Framework.Async;
using CrossX.Framework.Graphics;
using CrossX.Framework.IoC;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CrossX.Framework.Core
{
    public class MainLoop: IRedrawService
    {
        private Task _loopTask;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private AutoResetEvent _invalidatedEvent = new AutoResetEvent(false);

        private AutoResetEvent _updatedEvent = new AutoResetEvent(false);

        private volatile int redrawRequest = 0;
        Stopwatch _stopwatch = new Stopwatch();

        private readonly ICoreApplication coreApplication;

        public Action RedrawFunc { get; set; }
        private Size size = Size.Empty;

        private Dispatcher dispatcher = new Dispatcher();
        private SystemDispatcher systemDispatcher;
        private Sequencer sequencer = new Sequencer();

        public MainLoop(ICoreApplication coreApplication, Action redrawFunc, IScopeBuilder scopeBuilder, bool createSystemDispatcher)
        {
            RedrawFunc = redrawFunc;
            this.coreApplication = coreApplication;

            scopeBuilder
                .WithInstance(dispatcher).As<IDispatcher>()
                .WithInstance(sequencer).As<ISequencer>()
                .WithInstance(this).As<IRedrawService>();

            if(createSystemDispatcher)
            {
                systemDispatcher = new SystemDispatcher(RequestRedraw);
                scopeBuilder.WithInstance(systemDispatcher).As<ISystemDispatcher>();
            }
        }

        private async Task Loop(CancellationToken cancellationToken)
        {
            _stopwatch.Start();

            coreApplication.Run(size);
            RequestRedraw();

            var lastUpdateTimeSpan = _stopwatch.Elapsed;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    dispatcher.Process();
                    var currentTimeSpan = _stopwatch.Elapsed;
                    coreApplication.DoUpdate(currentTimeSpan - lastUpdateTimeSpan, this.size);
                    lastUpdateTimeSpan = currentTimeSpan;
                    _updatedEvent.Set();

                    if (redrawRequest > 0)
                    {
                        redrawRequest = 0;
                        RedrawFunc();
                        while (!_invalidatedEvent.WaitOne(20) && !cancellationToken.IsCancellationRequested) ;
                    }
                    else
                    {
                        await Task.Delay(50);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Debugger.Break();
                    throw;
                }
            }
        }

        public void OnPaintSurface(Canvas canvas)
        {
            if (_loopTask == null)
            {
                _loopTask = Task.Run(async () => await Loop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }

            if (size != canvas.Size)
            {
                size = canvas.Size;
                _updatedEvent.Reset();
                _invalidatedEvent.Set();
                _updatedEvent.WaitOne(100);
            }

            systemDispatcher?.Process();

            var currentTimeSpan = _stopwatch.Elapsed;
            coreApplication.DoRender(canvas);
            _invalidatedEvent.Set();
        }

        public void RequestRedraw() => redrawRequest++;

        public void Finish()
        {
            _cancellationTokenSource.Cancel(true);
            _loopTask.Wait();
        }
    }
}
