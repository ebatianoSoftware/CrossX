using CrossX.Framework.Core;
using System.Drawing;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{
    internal class AppToolStripRenderer : ToolStripProfessionalRenderer
    {
        public AppColorTable AppColorTable => (AppColorTable)ColorTable;
        public AppToolStripRenderer(IAppValues appValues): base(new AppColorTable(appValues))
        {
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            var tsMenuItem = e.Item as ToolStripMenuItem;
            if (tsMenuItem != null)
            {
                e.ArrowColor = tsMenuItem.Selected ? AppColorTable.ForegroundColorSelected : AppColorTable.ForegroundColor;
            }
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var tsMenuItem = e.Item as ToolStripMenuItem;
            if (tsMenuItem != null)
            {
                e.TextColor = tsMenuItem.Selected ? AppColorTable.ForegroundColorSelected : AppColorTable.ForegroundColor;
            }
            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if(e.Item is MenuItemEx mex)
            {
                if (mex.Icon.fontFamily == null) return;
                if (mex.Icon.iconText == null) return;

                using (var font = new Font(mex.Icon.fontFamily, e.ImageRectangle.Width - 2, GraphicsUnit.Pixel))
                {
                    using (var brush = new SolidBrush(AppColorTable.ForegroundColor))
                    {
                        e.Graphics.DrawString(mex.Icon.iconText, font, brush, e.ImageRectangle);
                    }
                }
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            
            if (e.Item is MenuItemEx mex)
            {

                Color color = (mex.Selected ? AppColorTable.ButtonSelectedHighlight : AppColorTable.ToolStripDropDownBackground);

                if(mex.Pressed)
                {
                    color = AppColorTable.ButtonPressedHighlight;
                }

                var rc = new Rectangle(Point.Empty, e.Item.Size);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, rc);
                }
            }
        }
    }
}
