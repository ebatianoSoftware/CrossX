using CrossX.Xml;

namespace CrossX.Forms.Transitions
{
    public abstract class Transform
    {
        private readonly bool inverted;

        protected Transform(XNodeAttributes attributes)
        {
            inverted = attributes.AsBoolean("Inverted");
        }

        public void Calculate(Vector2 origin, float timeNormalized, out Matrix transformation, out Color4 color)
        {
            CalculateTransform(origin, inverted ? (1-timeNormalized) : timeNormalized, out transformation, out color);
        }

        protected abstract void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix transformation, out Color4 color);
    }
}
