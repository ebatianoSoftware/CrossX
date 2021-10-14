namespace CrossX.Framework.Graphics
{
    public interface IEffectsFactory
    {
        IEffect CreateBlurEffect(SizeF blurSize);
    }
}
