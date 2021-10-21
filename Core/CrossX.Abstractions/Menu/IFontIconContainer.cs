namespace CrossX.Abstractions.Menu
{
    public interface IFontIconContainer
    {
        (string fontFamily, string iconText) Icon { get; }
    }
}
