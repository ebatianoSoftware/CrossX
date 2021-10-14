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

        public int MaxLength { get; set; } = int.MaxValue;

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

        private int selectionStart = 0;
        private int selectionLength = 0;

        private float selectionStartPosition = 0;
        private float selectionEndPosition = 0;

        private int initialSelectionStart = 0;

        private int nativeControlIsToRelease = 0;

        public TextBox(IUIServices services) : base(services)
        {
            buttonGesturesProcessor = new ButtonGesturesProcessor(
                state => CurrentState = state,
                null,
                onHoveredAction: OnHovered,
                onUpAction: OnHovered,
                onDownAction: OnDown,
                onMoveAction: OnMove
                );
        }

        private void OnHovered(Gesture gesture)
        {
            var bounds = ScreenBounds.Deflate(TextPadding);
            gesture.SetCursor = bounds.Contains(gesture.Position) ? CursorType.IBeam : CursorType.Default;
        }

        private void OnMove(Gesture gesture)
        {
            if(nativeTextBox != null && initialSelectionStart >= 0)
            {
                var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);

                var bounds = ScreenBounds.Deflate(TextPadding);
                var offset = gesture.Position.X - bounds.X + font.MeasureText("I", FontMeasure.Extended).Width / 2;
                var selection = font.BreakText(Text, offset);

                var selStart = initialSelectionStart;
                var selEnd = selection;

                if (selStart > selEnd)
                {
                    var end = selStart;
                    selStart = selEnd;
                    selEnd = end;
                }

                if(nativeTextBox.Selection.start != selStart || nativeTextBox.Selection.length != selEnd - selStart)
                {
                    nativeTextBox.Selection = (selStart, selEnd - selStart);
                }

                gesture.SetCursor = CursorType.IBeam;
            }
        }

        private async void OnDown(Gesture gesture)
        {
            try
            {
                nativeControlIsToRelease = 0;

                if (nativeTextBox == null)
                {
                    nativeTextBox = await Parent.Window.NativeWindow.CreateNativeTextBox(this, gesture.Position);
                }
                else
                {
                    nativeTextBox.Focus();
                }

                var bounds = ScreenBounds.Deflate(TextPadding);

                if (bounds.Contains(gesture.Position))
                {

                    var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
                    var offset = gesture.Position.X - bounds.X + font.MeasureText("I", FontMeasure.Extended).Width / 2;
                    var selection = font.BreakText(Text, offset);
                    nativeTextBox.Selection = (selection, 0);
                    initialSelectionStart = selection;
                    gesture.SetCursor = CursorType.IBeam;
                }
                else
                {
                    initialSelectionStart = -1;
                    nativeTextBox.Selection = (0, Text.Length);
                    gesture.SetCursor = CursorType.Default;
                }
                Services.Dispatcher.EnqueueAction(Invalidate);
                Invalidate();

            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            if(nativeControlIsToRelease > 0)
            {
                nativeControlIsToRelease--;
                if(nativeControlIsToRelease == 0)
                {
                    nativeTextBox?.Release();
                    nativeTextBox = null;
                }
            }

            if (nativeTextBox != null)
            {
                var sel = nativeTextBox.Selection;
                if (sel.start != selectionStart || sel.length != selectionLength)
                {
                    Invalidate();

                    selectionStart = sel.start;
                    selectionLength = sel.length;

                    if (selectionStart >= 0)
                    {
                        var textBefore = Text.Substring(0, selectionStart);
                        var textIn = Text.Substring(0, selectionStart + selectionLength);

                        var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);

                        selectionStartPosition = font.MeasureText(textBefore, FontMeasure).Width;
                        selectionEndPosition = font.MeasureText(textIn, FontMeasure).Width;
                    }
                    else
                    {
                        selectionStartPosition = -1;
                        selectionEndPosition = -1;
                    }
                }
            }
            else
            {
                selectionStart = -1;
                selectionLength = -1;

                selectionStartPosition = -1;
                selectionEndPosition = -1;
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

            if (selectionStartPosition >= 0)
            {
                var selectionRect = new RectangleF(bounds.X + selectionStartPosition - 0.5f, bounds.Y, selectionEndPosition - selectionStartPosition + 1f, bounds.Height);
                canvas.FillRect(selectionRect, SelectionColor);
            }

            if (!string.IsNullOrEmpty(text) )
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
            nativeControlIsToRelease = 2;
            Invalidate();
        }
    }
}
