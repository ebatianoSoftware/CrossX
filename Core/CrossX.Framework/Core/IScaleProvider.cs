namespace CrossX.Framework.Core
{
    public interface IScaleProvider
    {
        float CalculateScale(float currentScale, SizeF windowSize);
    }
}
