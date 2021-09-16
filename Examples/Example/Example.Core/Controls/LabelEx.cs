using CrossX.Framework;
using CrossX.Framework.Graphics;
using CrossX.Framework.UI.Controls;

namespace Example.Core.Controls
{
    public class LabelEx: Label
    {
        public Color NewColor { get; set; }

        public LabelEx(IFontManager fontManager): base(fontManager)
        {
        }
    }
}
