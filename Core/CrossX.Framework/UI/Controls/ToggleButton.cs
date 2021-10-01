using CrossX.Framework.Binding;
using CrossX.Framework.Graphics;

namespace CrossX.Framework.UI.Controls
{
    public class ToggleButton : Button
    {
        private Color checkedBackgroundColor;
        private Color checkedForegroundColor;
        private Color checkedBackgroundColorOver;
        private Color checkedForegroundColorOver;
        private Color checkedBackgroundColorPushed;
        private Color checkedForegroundColorPushed;

        private bool @checked;

        public Color CheckedBackgroundColor { get => checkedBackgroundColor; set => SetPropertyAndRedraw(ref checkedBackgroundColor, value); }
        public Color CheckedForegroundColor { get => checkedForegroundColor; set => SetPropertyAndRedraw(ref checkedForegroundColor, value); }

        public Color CheckedBackgroundColorOver { get => checkedBackgroundColorOver; set => SetPropertyAndRedraw(ref checkedBackgroundColorOver, value); }
        public Color CheckedForegroundColorOver { get => checkedForegroundColorOver; set => SetPropertyAndRedraw(ref checkedForegroundColorOver, value); }

        public Color CheckedBackgroundColorPushed { get => checkedBackgroundColorPushed; set => SetPropertyAndRedraw(ref checkedBackgroundColorPushed, value); }
        public Color CheckedForegroundColorPushed { get => checkedForegroundColorPushed; set => SetPropertyAndRedraw(ref checkedForegroundColorPushed, value); }

        [BindingMode(BindingMode.TwoWay)]
        public bool Checked { get => @checked; set => SetPropertyAndRedraw(ref @checked, value); }

        public ToggleButton(IUIServices services) : base(services)
        {

        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            if(Checked)
            {
                Color foregroundColor = CheckedForegroundColor;
                Color backgroundColor = CheckedBackgroundColor;

                switch (CurrentState)
                {
                    case ButtonState.Hover:
                        foregroundColor = CheckedForegroundColorOver;
                        backgroundColor = CheckedBackgroundColorOver;
                        break;

                    case ButtonState.Pushed:
                        foregroundColor = CheckedForegroundColorPushed;
                        backgroundColor = CheckedBackgroundColorPushed;
                        break;
                }

                if (!Enabled)
                {
                    foregroundColor = ForegroundColorDisabled;
                    backgroundColor = BackgroundColorDisabled;
                }

                RenderButton(canvas, foregroundColor, backgroundColor, opacity);
                return;
            }
            base.OnRender(canvas, opacity);
        }

        protected override void OnClick()
        {
            Checked = !Checked;
            base.OnClick();
        }
    }
}
