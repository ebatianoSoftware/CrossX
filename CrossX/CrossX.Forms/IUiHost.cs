using CrossX.Graphics2D;
using System.Drawing;

namespace CrossX.Forms
{
    public interface IUiHost
    {
        void BeginDraw();
        Rectangle TargetRect { get; }
        void EndDraw();
        Vector2 ScreenToUiUnits(Vector2 screenPoint);
        ITransform2D Transform2D { get; }
    }
}
