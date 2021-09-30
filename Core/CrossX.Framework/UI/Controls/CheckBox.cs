using CrossX.Framework.Binding;
using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;

namespace CrossX.Framework.UI.Controls
{
    public class CheckBox : TextBasedControl
    {
        private bool @checked;
        private Color checkedBoxColor;
        private Color checkedTickColor;
        private Color checkedBoxColorOver;
        private Color checkedTickColorOver;
        private Color checkedBoxColorPushed;
        private Color checkedTickColorPushed;

        private Color boxColor;
        private Color boxColorOver;
        private Color boxColorPushed;
        private ButtonState currentState;

        private bool enabled = true;
        private Length boxSize = new Length(90, Length.Type.Percent);
        private Length spacing;
        private Color tickColorPushed;
        private readonly ButtonGesturesProcessor buttonGesturesProcessor;

        [BindingMode(BindingMode.TwoWay)]
        public bool Checked { get => @checked; set => SetPropertyAndRedraw(ref @checked, value); }

        public Color BoxColor { get => boxColor; set => SetPropertyAndRedraw(ref boxColor, value); }
        public Color BoxColorOver { get => boxColorOver; set => SetPropertyAndRedraw(ref boxColorOver, value); }
        public Color BoxColorPushed { get => boxColorPushed; set => SetPropertyAndRedraw(ref boxColorPushed, value); }

        public Color CheckedBoxColor { get => checkedBoxColor; set => SetPropertyAndRedraw(ref checkedBoxColor, value); }
        public Color CheckedTickColor { get => checkedTickColor; set => SetPropertyAndRedraw(ref checkedTickColor, value); }

        public Color CheckedBoxColorOver { get => checkedBoxColorOver; set => SetPropertyAndRedraw(ref checkedBoxColorOver, value); }
        public Color CheckedTickColorOver { get => checkedTickColorOver; set => SetPropertyAndRedraw(ref checkedTickColorOver, value); }

        public Color CheckedBoxColorPushed { get => checkedBoxColorPushed; set => SetPropertyAndRedraw(ref checkedBoxColorPushed, value); }
        public Color CheckedTickColorPushed { get => checkedTickColorPushed; set => SetPropertyAndRedraw(ref checkedTickColorPushed, value); }

        public Color TickColorPushed { get => tickColorPushed; set => SetPropertyAndRedraw(ref tickColorPushed, value); }

        public Length BoxSize { get => boxSize; set => SetPropertyAndRecalcLayout(ref boxSize, value); }
        public Length Spacing { get => spacing; set => SetPropertyAndRecalcLayout(ref spacing, value); }
        public bool Enabled { get => enabled; set => SetPropertyAndRedraw(ref enabled, value); }

        public Drawable BoxDrawable { get; set; }
        public Drawable TickMarkDrawable { get; set; }

        protected ButtonState CurrentState
        {
            get => currentState;
            set => SetPropertyAndRedraw(ref currentState, value);
        }

        public CheckBox(IUIServices services) : base(services)
        {
            buttonGesturesProcessor = new ButtonGesturesProcessor(
                state => CurrentState = state,
                OnClick
                );
        }

        protected virtual void OnClick()
        {
            Checked = !Checked;
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);
            
            var boxSize = BoxSize.Calculate(ScreenBounds.Height);
            var spacing = Spacing.Calculate(ScreenBounds.Height);

            var font = Services.FontManager.FindFont(FontFamily, FontSize.Calculate(), FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);

            var boxBounds = ScreenBounds;

            if (HorizontalTextAlignment == Alignment.End)
            {
                boxBounds.Left = boxBounds.Right - boxSize;
                bounds.Right -= boxSize + spacing;
            }
            else
            {
                boxBounds.Width = boxSize;
                bounds.Left += boxSize + spacing;
            }

            boxBounds.Y = boxBounds.Center.Y - boxSize / 2;
            boxBounds.Height = boxSize;

            var boxColor = Checked ? CheckedBoxColor : BoxColor;
            var tickColor = CheckedTickColor;

            switch (CurrentState)
            {
                case ButtonState.Hover:
                    boxColor = Checked ? CheckedBoxColorOver : BoxColorOver;
                    tickColor = CheckedTickColorOver;
                    break;

                case ButtonState.Pushed:
                    boxColor = Checked ? CheckedBoxColorPushed : BoxColorPushed;
                    tickColor = Checked ? CheckedTickColorPushed : TickColorPushed;
                    break;
            }


            if (BoxDrawable != null)
            {
                BoxDrawable.Draw(canvas, boxBounds, boxColor * opacity);
            }
            else
            {
                canvas.FillRect(boxBounds, boxColor * opacity);
            }

            if(Checked || CurrentState == ButtonState.Pushed)
            {
                if (TickMarkDrawable != null)
                {
                    TickMarkDrawable.Draw(canvas, boxBounds, tickColor * opacity);
                }
                else
                {
                    canvas.FillRect(boxBounds.Deflate(3, 3), tickColor * opacity);
                }
            }

            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), ForegroundColor * opacity, FontMeasure);
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);
            var autoWidth = Width.IsAuto && HorizontalAlignment != Alignment.Stretch;

            if (autoWidth)
            {
                var boxSize = BoxSize.Calculate(size.Height);
                var spacing = Spacing.Calculate(size.Height);

                size.Width += boxSize + spacing;
            }
            return size;
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
    }
}
