using CrossX.Graphics2D.Text;
using System.Text;

namespace CrossX.Forms.Converters
{
    internal class StringBuilderToTextSourceConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if(value is StringBuilder sb)
            {
                return new TextSource(sb);
            }
            return new TextSource("");
        }
    }
}
