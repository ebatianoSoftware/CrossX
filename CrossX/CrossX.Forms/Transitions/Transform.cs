using CrossX.Forms.Xml;

namespace CrossX.Forms.Transitions
{
    public abstract class Transform
    {
        protected Transform(XNodeAttributes attributes)
        {
        }

        public void Calculate(Vector2 origin, float timeNormalized, out Matrix transformation, out Color4 color)
        {
            CalculateTransform(origin, timeNormalized, out transformation, out color);
        }

        protected abstract void CalculateTransform(Vector2 origin, float timeNormalized, out Matrix transformation, out Color4 color);
    }
}
