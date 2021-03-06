using CrossX.Forms.Converters;
using CrossX.Xml;
using System.Numerics;

namespace CrossX.Forms.Transitions
{
    public class TintTransform : Transform
    {
        public Color4 Tint { get; }

        public TintTransform(XNodeAttributes attributes): base(attributes)
        {
            Tint = (Color4)StringToColorConverter.Instance.Convert(attributes.AsString("Tint"));
        }

        protected override void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix4x4 transformation, out Color4 color)
        {
            transformation = Matrix4x4.Identity;
            color = new Color4
            {
                Rf = (1 - timeNormalized) + timeNormalized * Tint.Rf,
                Gf = (1 - timeNormalized) + timeNormalized * Tint.Gf,
                Bf = (1 - timeNormalized) + timeNormalized * Tint.Bf,
                Af = (1 - timeNormalized) + timeNormalized * Tint.Af
            };
        }
    }
}
