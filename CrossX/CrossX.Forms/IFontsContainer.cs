using CrossX.Graphics2D.Text;

namespace CrossX.Forms
{
    public interface IFontsContainer
    {
        Font Find(string fontFace, float size, FontStyle fontStyle);
    }
}
