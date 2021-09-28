using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CrossX.Framework.UI.Controls
{
    public enum ButtonState
    {
        Normal,
        Hover,
        Pushed
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

        public Color ForegroundColorDisabled { get => foregroundColorDisabled; set => SetProperty(ref foregroundColorDisabled, value); }

        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        private ButtonState CurrentState
        {
            get => currentState;
            set
            {
                if (currentState != value)
                {
                    currentState = value;
                    Services.RedrawService.RequestRedraw();
                }
            }
        }

        private PointerId lockedPointer = PointerId.None;
        private HashSet<PointerId> hoverPointers = new HashSet<PointerId>();

        private ICommand command;
        private object commandParameter;

        private Color backgroundColorPushed = Color.Transparent;
        private Color backgroundColorOver = Color.Transparent;
        private Color backgroundColorDisabled = Color.Gray;

        private Color foregroundColorPushed = Color.Black;
        private Color foregroundColorOver = Color.Black;
        private Color foregroundColorDisabled = Color.DarkGray;
        private bool enabled = true;
        private ButtonState currentState = ButtonState.Normal;

        public Button(IUIServices services) : base(services)
        {
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

            if (BackgroundDrawable == null)
            {
                canvas.FillRect(ScreenBounds, backgroundColor * opacity);
            }
            else
            {
                BackgroundDrawable.Draw(canvas, ScreenBounds, backgroundColor * opacity);
            }
            
            var font = Services.FontManager.FindFont(FontFamily, FontSize, FontWeight, FontItalic);
            var bounds = ScreenBounds.Deflate(TextPadding);
            canvas.DrawText(Text, font, bounds, Utils.GetTextAlign(HorizontalTextAlignment, VerticalTextAlignment), foregroundColor * opacity, FontMeasure);
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
                            CurrentState = ButtonState.Pushed;
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
                            CurrentState = ButtonState.Hover;
                            Command?.Execute(CommandParameter);
                        }
                        else
                        {
                            CurrentState = ButtonState.Normal;
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

                        ButtonState newState = hoverPointers.Count > 0 ? ButtonState.Hover : ButtonState.Normal;

                        if (newState != CurrentState)
                        {
                            CurrentState = newState;
                            return true;
                        }
                    }
                    else if (lockedPointer == gesture.PointerId)
                    {
                        if (ScreenBounds.Contains(gesture.Position))
                        {
                            CurrentState = ButtonState.Pushed;
                        }
                        else
                        {
                            CurrentState = ButtonState.Normal;
                        }
                        return true;
                    }
                    break;

                case GestureType.CancelPointer:
                    if (lockedPointer == gesture.PointerId)
                    {
                        CurrentState = ButtonState.Normal;
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
                case nameof(Visible):
                    lockedPointer = PointerId.None;
                    CurrentState = ButtonState.Normal;
                    Services.RedrawService.RequestRedraw();
                    break;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs _)
        {
            Enabled = command?.CanExecute(CommandParameter) ?? true;
        }
    }
}
