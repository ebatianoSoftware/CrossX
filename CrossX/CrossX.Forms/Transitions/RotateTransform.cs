using CrossX.Xml;

namespace CrossX.Forms.Transitions
{
    public class RotateTransform : Transform
    {
        public float Angle { get; }

        public RotateTransform(XNodeAttributes attributes): base(attributes)
        {
            Angle = MathHelper.Pi * (float)attributes.AsDouble("Angle", 0) / 180f;
        }

        protected override void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix transformation, out Color4 color)
        {
            transformation =
                Matrix.CreateTranslation(-origin.X, -origin.Y, 0) *
                Matrix.CreateRotationZ(Angle * timeNormalized) *
                Matrix.CreateTranslation(origin.X, origin.Y, 0);

            color = Color4.White;
        }
    }
}
