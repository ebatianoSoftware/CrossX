using CrossX.Framework.Binding;
using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.Input.TextInput;
using System;

namespace CrossX.Framework.UI.Controls
{
    public class TextBox : Label, INativeTextBoxControl
    {
        private Drawable frameDrawable;
        private Color frameColor;
        private Color frameColorOver;
        private Color frameColorActive;
        private Color backgroundColorDisabled;
        private bool enabled = true;
        private readonly ButtonGesturesProcessor buttonGesturesProcessor;

        public Drawable FrameDrawable { get => frameDrawable; set => SetPropertyAndRedraw(ref frameDrawable, value); }

        public Color FrameColor { get => frameColor; set => SetPropertyAndRedraw(ref frameColor, value); }
        public Color FrameColorOver { get => frameColorOver; set => SetPropertyAndRedraw(ref frameColorOver, value); }
        public Color FrameColorActive { get => frameColorActive; set => SetPropertyAndRedraw(ref frameColorActive, value); }
        public Color BackgroundColorDisabled { get => backgroundColorDisabled; set => SetProperty(ref backgroundColorDisabled, value); }

        public Color HintColor { get => hintColor; set => SetProperty(ref hintColor, value); }
        public Color HintColorDisabled { get => hintColorDisabled; set => SetProperty(ref hintColorDisabled, value); }

        public Color SelectionColor { get => selectionColor; set => SetProperty(ref selectionColor, value); }

        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        [BindingMode(BindingMode.TwoWay)]
        public override string Text { get => base.Text; set => base.Text = value; }

        public string Hint { get => hint; set => SetPropertyAndRedraw(ref hint, value); }

        protected ButtonState CurrentState
        {
            get => currentState;
            set => SetPropertyAndRedraw(ref currentState, value);
        }

        Rectangle INativeTextBoxControl.Bounds => ScreenBounds.Deflate(TextPadding).GetPixelsRect();
        Color INativeTextBoxControl.TextColor => ForegroundColor;

        string INativeTextBoxControl.FontFamily
        {
            get
            {
                var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
                return font.FamilyName;
            }
        }

        private ButtonState currentState = ButtonState.Normal;
        private string hint;
        private Color hintColor;
        private Color hintColorDisabled;

        private INativeTextBox nativeTextBox = null;
        private Color selectionColor;

        public TextBox(IUIServices services) : base(services)
        {
            buttonGesturesProcessor = new ButtonGesturesProcessor(
                state => CurrentState = state,
                null,
                onHoveredAction: g => g.SetCursor = CursorType.IBeam,
                onUpAction: g => g.SetCursor = CursorType.IBeam,
                onDownAction: OnDown,
                onMoveAction: g => g.SetCursor = CursorType.IBeam
                );
        }

        private async void OnDown(Gesture gesture)
        {
            gesture.SetCursor = CursorType.IBeam;

            try
            {
                if (nativeTextBox == null)
                {
                    nativeTextBox = await Parent.Window.NativeWindow.CreateNativeTextBox(this, gesture.Position);
                    Services.Dispatcher.EnqueueAction(Invalidate);
                    Invalidate();
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            var backgroundColor = BackgroundColor;
            var frameColor = FrameColor;
            var foregroundColor = string.IsNullOrEmpty(Text) ? HintColor : ForegroundColor;

            switch (CurrentState)
            {
                case ButtonState.Hover:
                case ButtonState.Pushed:
                    frameColor = FrameColorOver;
                    break;
            }

            if (!Enabled)
            {
                backgroundColor = BackgroundColorDisabled;
                frameColor = ForegroundColorDisabled;
                foregroundColor = string.IsNullOrEmpty(Text) ? HintColorDisabled : ForegroundColorDisabled;
            }

            if (nativeTextBox != null)
            {
                frameColor = FrameColorActive;
            }

            if (BackgroundDrawable == null)
            {
                canvas.FillRect(ScreenBounds, backgroundColor * opacity);
            }
            else
            {
                BackgroundDrawable.Draw(canvas, ScreenBounds, backgroundColor * opacity);
            }

            if (FrameDrawable != null)
            {
                FrameDrawable.Draw(canvas, ScreenBounds, frameColor * opacity);
            }
            else
            {
                canvas.DrawRect(ScreenBounds, frameColor * opacity, 1);
            }

            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            var text = string.IsNullOrEmpty(Text) ? Hint : Text;

            if (!string.IsNullOrEmpty(text) && nativeTextBox == null)
            {
                canvas.DrawText(text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), foregroundColor * opacity, FontMeasure);
            }
        }

        protected override bool OnPreviewGesture(Gesture gesture)
        {
            buttonGesturesProcessor.PreviewGesture(gesture, ScreenBounds);
            return false;
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
                    Invalidate();
                    break;

                case nameof(BackgroundDrawable):
                    break;
            }
        }

        void INativeTextBoxControl.OnLostFocus()
        {
            nativeTextBox.Release();
            nativeTextBox = null;
            Invalidate();
        }
    }
}
