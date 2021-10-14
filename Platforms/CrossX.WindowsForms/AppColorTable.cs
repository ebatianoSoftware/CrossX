using CrossX.Framework.Core;
using System.Drawing;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{

    internal class AppColorTable: ProfessionalColorTable
    {
        private readonly IAppValues appValues;

        public AppColorTable(IAppValues appValues)
        {
            this.appValues = appValues;
        }
        
        private Color GetColor(string name) => ((Framework.Color)appValues.GetValue(name)).ToDrawing();

        public override Color ToolStripDropDownBackground => GetColor("SystemPanelBackgroundColor");

        public override Color ImageMarginGradientBegin => GetColor("SystemPanelBackgroundColor");

        public override Color ImageMarginGradientMiddle => GetColor("SystemPanelBackgroundColor");

        public override Color ImageMarginGradientEnd => GetColor("SystemPanelBackgroundColor");

        public override Color MenuBorder => GetColor("SystemPanelBackgroundColor");

        public override Color MenuItemBorder => GetColor("SystemPanelBackgroundColor");

        public override Color MenuItemSelected => GetColor("SystemButtonBackgroundColor");

        public override Color MenuStripGradientBegin => GetColor("SystemPanelBackgroundColor");

        public override Color MenuStripGradientEnd => GetColor("SystemPanelBackgroundColor");

        public override Color MenuItemSelectedGradientBegin => GetColor("SystemButtonAccentBackgroundColorPushed");

        public override Color MenuItemSelectedGradientEnd => GetColor("SystemButtonAccentBackgroundColorPushed");

        public override Color MenuItemPressedGradientBegin => GetColor("SystemButtonAccentBackgroundColorPushed");

        public override Color MenuItemPressedGradientEnd => GetColor("SystemButtonAccentBackgroundColorPushed");

        public override Color MenuItemPressedGradientMiddle => GetColor("SystemButtonAccentBackgroundColorPushed");

        public override Color ButtonSelectedHighlight => GetColor("SystemButtonAccentBackgroundColorOver");
        public override Color ButtonPressedHighlight => GetColor("SystemButtonAccentBackgroundColorPushed");

        public override Color ToolStripBorder => GetColor("SystemPanelBackgroundColor");

        public Color ForegroundColor => GetColor("SystemButtonForegroundColor");

        public override Color SeparatorLight => GetColor("SystemPanelBackgroundColorLevel2");
        public override Color SeparatorDark => GetColor("SystemPanelBackgroundColorLevel2");

        public override Color OverflowButtonGradientBegin => GetColor("SystemForegroundColor");
        public override Color OverflowButtonGradientEnd => GetColor("SystemForegroundColor");
        public override Color OverflowButtonGradientMiddle => GetColor("SystemForegroundColor");

        public Color ForegroundColorSelected => GetColor("SystemButtonForegroundColorOver");
    }
}
