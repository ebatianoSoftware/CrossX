﻿using System.Numerics;

namespace CrossX.Framework.Graphics
{
    public abstract class Canvas : Disposable
    {
        public virtual BlendMode BlendMode { get; set; } = BlendMode.AlphaBlend;
        public virtual IEffect Effect { get; set; }

        public abstract Size Size { get; }
        public abstract int SaveState();
        public abstract void Restore();
        public abstract void FillRect(RectangleF rect, Color color);
        public abstract void DrawRect(RectangleF rect, Color color, float thickness);

        public abstract void DrawArc(RectangleF rect, float angle, float sweep, Color color, bool close, float thickness);
        public abstract void FillArc(RectangleF rect, float angle, float sweep, Color color);
        public abstract void DrawLine(Vector2 p1, Vector2 p2, Color color, float thickness);

        public abstract void FillRoundRect(RectangleF rect, Vector2 roundness, Color color);
        public abstract void DrawRoundRect(RectangleF rect, Vector2 roundness, Color color, float thickness);

        public abstract void Restore(int count);
        public abstract void Transform(Matrix3x2 transform);
        public abstract void ClipRect(RectangleF rect);
        public abstract void Clear(Color color);
        public abstract void DrawTriangles(VertexBuffer vertexBuffer, Image image);
        public abstract Vector2 DrawText(string text, Font font, RectangleF target, TextAlign align, Color color, FontMeasure fontMeasure = FontMeasure.Extended);
        public abstract void DrawImage(Image image, RectangleF target, RectangleF source, float opacity);
        public abstract void DrawEllipse(RectangleF rect, Color color, float thickness);
        public abstract void FillEllipse(RectangleF rect, Color color);
    }
}
