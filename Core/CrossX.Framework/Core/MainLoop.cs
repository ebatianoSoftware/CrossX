using CrossX.Abstractions.Async;
using CrossX.Abstractions.IoC;
using CrossX.Framework.Async;
using CrossX.Framework.Graphics;
using CrossX.Framework.Utility;
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
        private Size size;

        private readonly Dispatcher dispatcher = new Dispatcher();
        private SystemDispatcher systemDispatcher;
        private Sequencer sequencer = new Sequencer();
        
        public IDispatcher Dispatcher => dispatcher;

        public MainLoop(ICoreApplication coreApplication, Action redrawFunc, IScopeBuilder scopeBuilder, float dpi)
        {
            UiUnit.PixelsPerUnit = dpi / 96f;

            RedrawFunc = redrawFunc;
            this.coreApplication = coreApplication;

            Sequence.CreateSequenceFunc = SequenceImpl.Create;
            Sequence.NextFrameSequence = SequenceImpl.Create(0, 0, null, null);

            scopeBuilder
                .WithInstance(dispatcher).As<IDispatcher>()
                .WithInstance(sequencer).As<ISequencer>()
                .WithInstance(this).As<IRedrawService>()
                .WithCrossTypes();

            systemDispatcher = new SystemDispatcher();
            scopeBuilder.WithInstance(systemDispatcher).As<ISystemDispatcher>();
            
        }

        public void ProcessSystemDispatcher() => systemDispatcher.Process();

        private void Loop(CancellationToken cancellationToken)
        {
            dispatcher.DispatcherThread = Thread.CurrentThread;

            _stopwatch.Start();
            coreApplication.Run();

            var lastUpdateTimeSpan = _stopwatch.Elapsed;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    dispatcher.Process();
                    var currentTimeSpan = _stopwatch.Elapsed;
                    var timeDelta = currentTimeSpan - lastUpdateTimeSpan;
                    sequencer.Update(timeDelta);
                    coreApplication.DoUpdate(timeDelta, size);
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
                        dispatcher.Wait(16);
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
                _loopTask = Task.Run(()=>Loop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }

            if (size != canvas.Size)
            {
                size = canvas.Size;
                _updatedEvent.Reset();
                _invalidatedEvent.Set();
                _updatedEvent.WaitOne(10);
                RequestRedraw();
            }

            if (systemDispatcher != null)
            {
                systemDispatcher.DispatcherThread = Thread.CurrentThread;
            }
            systemDispatcher?.Process();

            var currentTimeSpan = _stopwatch.Elapsed;
            coreApplication.DoRender(canvas);
            _invalidatedEvent.Set();
        }

        public void Initialize()
        {
            _updatedEvent.Reset();

            if (_loopTask == null)
            {
                _loopTask = Task.Run(() => Loop(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            }

            _updatedEvent.WaitOne(500);
            systemDispatcher?.Process();
        }

        public void RequestRedraw()
        {
            redrawRequest++;
            dispatcher.Touch();
        }

        public void Finish()
        {
            _cancellationTokenSource.Cancel(true);
            _loopTask.Wait();
        }
    }
}
