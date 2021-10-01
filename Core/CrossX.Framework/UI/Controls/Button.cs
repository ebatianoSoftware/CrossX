using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.Styles;
using System;
using System.Windows.Input;

namespace CrossX.Framework.UI.Controls
{
    public enum ButtonState
    {
        Normal,
        Hover,
        Pushed
    }

    public class DefaultButton : Button
    {
        public DefaultButton(IUIServices services) : base(services)
        {
        }

        protected override void ApplyDefaultStyle()
        {
            base.ApplyDefaultStyle();

            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonAccentBackgroundColor) is Color bgColor) BackgroundColor = bgColor;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonAccentBackgroundColorOver) is Color bgColorOver) BackgroundColorOver = bgColorOver;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonAccentBackgroundColorPushed) is Color bgColorPushed) BackgroundColorPushed = bgColorPushed;
        }
    }

    public class Button : TextBasedControl
    {
        public ICommand Command
        {
            get => command;
            set
            {
                if (value != command)
                {
                    if (command != null)
                    {
                        command.CanExecuteChanged -= Command_CanExecuteChanged;
                    }
                    SetProperty(ref command, value);

                    if (command != null)
                    {
                        command.CanExecuteChanged += Command_CanExecuteChanged;
                    }
                }
            }
        }

        public object CommandParameter { get => commandParameter; set => SetProperty(ref commandParameter, value); }
        public Color BackgroundColorPushed { get => backgroundColorPushed; set => SetProperty(ref backgroundColorPushed, value); }
        public Color BackgroundColorOver { get => backgroundColorOver; set => SetProperty(ref backgroundColorOver, value); }

        public Color BackgroundColorDisabled { get => backgroundColorDisabled; set => SetProperty(ref backgroundColorDisabled, value); }

        public Color ForegroundColorPushed { get => foregroundColorPushed; set => SetProperty(ref foregroundColorPushed, value); }
        public Color ForegroundColorOver { get => foregroundColorOver; set => SetProperty(ref foregroundColorOver, value); }

        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        protected ButtonState CurrentState
        {
            get => currentState;
            set => SetPropertyAndRedraw(ref currentState, value);
        }

        private ICommand command;
        private object commandParameter;

        private Color backgroundColorPushed = Color.Transparent;
        private Color backgroundColorOver = Color.Transparent;
        private Color backgroundColorDisabled = Color.Gray;

        private Color foregroundColorPushed = Color.Black;
        private Color foregroundColorOver = Color.Black;
        private bool enabled = true;
        private ButtonState currentState = ButtonState.Normal;

        private readonly ButtonGesturesProcessor buttonGesturesProcessor;

        public Button(IUIServices services) : base(services)
        {
            buttonGesturesProcessor = new ButtonGesturesProcessor(
                state => CurrentState = state,
                OnClick
                );

            ApplyDefaultStyle();
        }

        protected virtual void OnClick()
        {
            Command?.Execute(CommandParameter);
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            Color foregroundColor = ForegroundColor;
            Color backgroundColor = BackgroundColor;

            switch (CurrentState)
            {
                case ButtonState.Hover:
                    foregroundColor = ForegroundColorOver;
                    backgroundColor = BackgroundColorOver;
                    break;

                case ButtonState.Pushed:
                    foregroundColor = ForegroundColorPushed;
                    backgroundColor = BackgroundColorPushed;
                    break;
            }

            if (!enabled)
            {
                foregroundColor = ForegroundColorDisabled;
                backgroundColor = BackgroundColorDisabled;
            }

            RenderButton(canvas, foregroundColor, backgroundColor, opacity);
        }

        protected void RenderButton(Canvas canvas, Color foregroundColor, Color backgroundColor, float opacity)
        {
            if (BackgroundDrawable == null)
            {
                canvas.FillRect(ScreenBounds, backgroundColor * opacity);
            }
            else
            {
                BackgroundDrawable.Draw(canvas, ScreenBounds, backgroundColor * opacity);
            }

            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), foregroundColor * opacity, FontMeasure);
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            if (buttonGesturesProcessor.ProcessGesture(gesture, ScreenBounds, Enabled)) return true;
            return base.OnProcessGesture(gesture);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Enabled):
                case nameof(Visible):
                    buttonGesturesProcessor.Reset();
                    Services.RedrawService.RequestRedraw();
                    break;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs _)
        {
            Enabled = command?.CanExecute(CommandParameter) ?? true;
        }

        protected override void ApplyDefaultStyle()
        {
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonBackgroundColor) is Color bgColor) BackgroundColor = bgColor;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonBackgroundColorOver) is Color bgColorOver) BackgroundColorOver = bgColorOver;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonBackgroundColorPushed) is Color bgColorPushed) BackgroundColorPushed = bgColorPushed;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonBackgroundColorDisabled) is Color bgColorDisabled) BackgroundColorDisabled = bgColorDisabled;

            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonForegroundColor) is Color fgColor) ForegroundColor = fgColor;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonForegroundColorOver) is Color fgColorOver) ForegroundColorOver = fgColorOver;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonForegroundColorPushed) is Color fgColorPushed) ForegroundColorPushed = fgColorPushed;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonForegroundColorDisabled) is Color fgColorDisabled) ForegroundColorDisabled = fgColorDisabled;

            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonTextFontFamily) is string fontFamily) FontFamily = fontFamily;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonTextFontSize) is Length fontSize) FontSize = fontSize;
            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonTextFontWeight) is FontWeight fontWeight) FontWeight = fontWeight;

            if (Services.AppValues.GetValue(ThemeValueKey.SystemButtonTextPadding) is Thickness textPadding) TextPadding = textPadding;

            BackgroundDrawable = Services.AppValues.GetResource(ResourceValueKey.SystemButtonBackgroundDrawable) as Drawable;

            FontMeasure = FontMeasure.Strict;
        }
    }
}
