using CrossX.Graphics;
using CrossX.Graphics2D;
using System.Drawing;

namespace CrossX.Forms
{
    public interface IUiHost
    {
        void Update();
        void BeginDraw();
        Rectangle TargetRect { get; }
        void EndDraw();
        ITransform2D Transform2D { get; }
        float ScaleToPixel { get; }
        TextureFilter DesiredTextureFilter { get; }
    }
}
