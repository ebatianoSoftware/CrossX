using CrossX.Abstractions.IoC;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using System;
using System.Numerics;

namespace CrossX.Framework.Core
{
    public interface ICoreApplication: IDisposable
    {
        void Run();
        void DoUpdate(TimeSpan ellapsedTime, Size size);
        void DoRender(Canvas canvas);
        void Initialize(IServicesProvider servicesProvider);

        void OnPointerDown(PointerId pointerId, Vector2 position);
        void OnPointerUp(PointerId pointerId, Vector2 position);
        void OnPointerMove(PointerId pointerId, Vector2 position);
        void OnPointerCancel(PointerId pointerId);
    }
}
