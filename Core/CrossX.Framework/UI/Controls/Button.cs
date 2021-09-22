using CrossX.Framework.Drawables;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CrossX.Framework.UI.Controls
{
    public class Button : TextBasedControl
    {
        enum State
        {
            Normal,
            Hover,
            Pushed
        }

        public ICommand Command 
        { 
            get => command;
            set
            {
                if(value != command)
                {
                    if(command != null)
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
        public Drawable BackgroundDrawable { get => backgroundDrawable; set => SetProperty(ref backgroundDrawable, value); }

        public Color BackgroundColorPushed { get => backgroundColorPushed; set => SetProperty(ref backgroundColorPushed, value); }
        public Color BackgroundColorOver { get => backgroundColorOver; set => SetProperty(ref backgroundColorOver, value); }

        public Color BackgroundColorDisabled { get => backgroundColorDisabled; set => SetProperty(ref backgroundColorDisabled, value); }

        public Color ForegroundColorPushed { get => foregroundColorPushed; set => SetProperty(ref foregroundColorPushed, value); }
        public Color ForegroundColorOver { get => foregroundColorOver; set => SetProperty(ref foregroundColorOver, value); }

        public Color ForegroundColorDisabled { get => foregroundColorDisabled; set => SetProperty(ref foregroundColorDisabled, value); }

        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        private State state = State.Normal;
        private PointerId lockedPointer = PointerId.None;

        private HashSet<PointerId> hoverPointers = new HashSet<PointerId>();

        private ICommand command;
        private object commandParameter;
        private Drawable backgroundDrawable;

        private Color backgroundColorPushed = Color.Transparent;
        private Color backgroundColorOver = Color.Transparent;
        private Color backgroundColorDisabled = Color.Gray;

        private Color foregroundColorPushed = Color.Black;
        private Color foregroundColorOver = Color.Black;
        private Color foregroundColorDisabled = Color.DarkGray;
        private bool enabled = true;
        private readonly IRedrawService redrawService;

        public Button(IFontManager fontManager, IRedrawService redrawService) : base(fontManager)
        {
            BackgroundDrawable = new RectangleDrawable
            {
                Rx = 2,
                Ry = 2,
                FillColor = Color.White * 0.5f,
                //StrokeColor = Color.Gray,
                //StrokeThickness = 2
            };
            this.redrawService = redrawService;
        }

        protected override void OnRender(Canvas canvas)
        {
            Color foregroundColor = ForegroundColor;
            Color backgroundColor = BackgroundColor;

            switch (state)
            {
                case State.Hover:
                    foregroundColor = ForegroundColorOver;
                    backgroundColor = BackgroundColorOver;
                    break;

                case State.Pushed:
                    foregroundColor = ForegroundColorPushed;
                    backgroundColor = BackgroundColorPushed;
                    break;
            }

            if(!enabled)
            {
                foregroundColor = ForegroundColorDisabled;
                backgroundColor = BackgroundColorDisabled;
            }

            BackgroundDrawable?.Draw(canvas, ScreenBounds, backgroundColor);

            var font = FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), foregroundColor, FontMeasure);
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            switch (gesture.GestureType)
            {
                case GestureType.PointerDown:
                    if (lockedPointer == PointerId.None && Enabled)
                    {
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            lockedPointer = gesture.PointerId;
                            state = State.Pushed;
                            return true;
                        }
                    }
                    break;

                case GestureType.PointerUp:
                    if (lockedPointer == gesture.PointerId)
                    {
                        lockedPointer = PointerId.None;
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            state = State.Hover;
                            Command?.Execute(CommandParameter);
                        }
                        else
                        {
                            state = State.Normal;
                        }
                        return true;
                    }
                    break;

                case GestureType.PointerMove:
                    if (lockedPointer == PointerId.None)
                    {
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            if (!hoverPointers.Contains(gesture.PointerId))
                            {
                                hoverPointers.Add(gesture.PointerId);
                            }
                        }
                        else
                        {
                            hoverPointers.Remove(gesture.PointerId);
                        }

                        State newState = hoverPointers.Count > 0 ? State.Hover : State.Normal;

                        if (newState != state)
                        {
                            state = newState;
                            return true;
                        }
                    }
                    else if (lockedPointer == gesture.PointerId)
                    {
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            state = State.Pushed;
                        }
                        else
                        {
                            state = State.Normal;
                        }
                        return true;
                    }
                    break;

                case GestureType.CancelPointer:
                    if (lockedPointer == gesture.PointerId)
                    {
                        state = State.Normal;
                        lockedPointer = PointerId.None;
                        return true;
                    }
                    break;
            }
            return base.OnProcessGesture(gesture);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Enabled):
                    lockedPointer = PointerId.None;
                    state = State.Normal;
                    redrawService.RequestRedraw();
                    break;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs _)
        {
            Enabled = command?.CanExecute(CommandParameter) ?? true;
        }
    }
}
