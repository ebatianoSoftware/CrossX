using CrossX.Xml;

namespace CrossX.Forms.Transitions
{
    public class ScaleTransform : Transform
    {
        public Vector2 Scale { get; }

        public ScaleTransform(XNodeAttributes attributes): base(attributes)
        {
            attributes.Parse2Double("Scale", out var x, out var y);
            Scale = new Vector2((float)x, (float)y);
        }

        protected override void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix transformation, out Color4 color)
        {
            var scale = Vector2.One * (1 - timeNormalized) + Scale * timeNormalized;
            transformation =
                Matrix.CreateTranslation(-origin.X, -origin.Y, 0) *
                Matrix.CreateScale(scale.X, scale.Y, 1) *
                Matrix.CreateTranslation(origin.X, origin.Y, 0);
            color = Color4.White;
        }
    }
}
