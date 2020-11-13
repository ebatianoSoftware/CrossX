using CrossX.Content;
using CrossX.Forms.Values;
using CrossX.Graphics;
using System.Drawing;

namespace CrossX.Forms.Converters
{
    public class StringToImageSourceConverter : IValueConverter
    {
        private readonly IContentManager contentManager;

        public StringToImageSourceConverter(IContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public object Convert(object value)
        {
            if (value is string text)
            {
                var texture = contentManager.Get<Texture2D>(text);
                return new ImageSource(texture);
            }
            return new ImageSource(null, Rectangle.Empty);
        }
    }
}
