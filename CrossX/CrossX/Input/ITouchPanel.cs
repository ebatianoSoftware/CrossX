using System;

namespace CrossX.Input
{
    public interface ITouchPanel
    {
        event Action<TouchPoint> PointerDown;
        event Action<TouchPoint> PointerUp;
        event Action<TouchPoint> PointerMove;
        event Action<TouchPoint> PointerRemoved;
        event Action<long, object> PointerCaptured;

        void CapturePointer(long id, object capturedBy);
    }
}
