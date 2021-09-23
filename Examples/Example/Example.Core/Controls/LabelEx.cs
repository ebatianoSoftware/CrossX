using CrossX.Framework;
using CrossX.Framework.Graphics;
using CrossX.Framework.UI;
using CrossX.Framework.UI.Controls;

namespace Example.Core.Controls
{
    public class LabelEx: Label
    {
        public Color NewColor { get; set; }

        public LabelEx(IUIServices services): base(services)
        {
        }
    }
}
