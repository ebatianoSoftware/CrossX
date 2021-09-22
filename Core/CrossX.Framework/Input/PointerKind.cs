using System;

namespace CrossX.Framework.Input
{
    [Flags]
    public enum PointerKind
    {
        MousePointer = 1,
        MouseLeftButton = 2,
        MouseMiddleButton = 4,
        MouseRightButton = 8,
        Touch = 16,
        MouseKinds = MousePointer | MouseLeftButton | MouseRightButton | MouseMiddleButton
    }
}
