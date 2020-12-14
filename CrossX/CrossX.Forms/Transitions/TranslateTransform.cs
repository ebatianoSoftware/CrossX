using CrossX.Xml;
using System.Numerics;

namespace CrossX.Forms.Transitions
{
    public class TranslateTransform : Transform
    {
        public Vector2 Translation { get; }

        public TranslateTransform(XNodeAttributes attributes): base(attributes)
        {
            attributes.Parse2Double("Translation", out var x, out var y);
            Translation = new Vector2((float)x, (float)y);
        }

        protected override void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix4x4 transformation, out Color4 color)
        {
            transformation = Matrix4x4.CreateTranslation( new Vector3(Translation * timeNormalized, 0));
            color = Color4.White;
        }
    }
}
