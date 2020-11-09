using CrossX.Graphics2D.Text;

namespace CrossX.Forms.Converters
{
    internal class StringToTextSourceConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if (value is string text)
            {
                return new TextSource(text);
            }
            return new TextSource("");
        }
    }
}
