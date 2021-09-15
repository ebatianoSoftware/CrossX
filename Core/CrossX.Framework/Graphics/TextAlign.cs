using System;

namespace CrossX.Framework.Graphics
{
    [Flags]
    public enum TextAlign
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Top = 0,
        Middle = 4,
        Bottom = 8
    }

    public enum HorizontalTextAlignment
    {
        Left = TextAlign.Left,
        Center = TextAlign.Center,
        Right = TextAlign.Right
    }

    public enum VerticalTextAlignment
    {
        Top = TextAlign.Top,
        Middle = TextAlign.Middle,
        Bottom = TextAlign.Bottom
    }

}
