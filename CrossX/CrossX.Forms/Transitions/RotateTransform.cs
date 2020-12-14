using CrossX.Xml;
using System;
using System.Numerics;

namespace CrossX.Forms.Transitions
{
    public class RotateTransform : Transform
    {
        public float Angle { get; }

        public RotateTransform(XNodeAttributes attributes): base(attributes)
        {
            Angle = (float)Math.PI * (float)attributes.AsDouble("Angle", 0) / 180f;
        }

        protected override void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix4x4 transformation, out Color4 color)
        {
            transformation =
                Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
                Matrix4x4.CreateRotationZ(Angle * timeNormalized) *
                Matrix4x4.CreateTranslation(origin.X, origin.Y, 0);

            color = Color4.White;
        }
    }
}
