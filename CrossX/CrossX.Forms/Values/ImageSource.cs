using CrossX.Graphics;
using System.Drawing;

namespace CrossX.Forms.Values
{
    public struct ImageSource
    {
        public ImageSource(Texture2D texture, Rectangle sourceRect)
        {
            SourceRect = sourceRect;
            Texture = texture;
        }

        public Rectangle SourceRect { get; }
        public Texture2D Texture { get; }
    }
}
