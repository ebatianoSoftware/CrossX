namespace CrossX.Framework.UI
{
    public interface IViewParent
    {
        void InvalidateLayout();
        RectangleF ScreenBounds { get; }
    }
}
