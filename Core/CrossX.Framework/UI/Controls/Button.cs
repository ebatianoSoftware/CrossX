﻿using CrossX.Abstractions.Input;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
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

    public class Button : TextBasedControl, IFocusable
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
                        Command_CanExecuteChanged(this, EventArgs.Empty);
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
        public bool FocusOnOver { get; set; }
        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        public bool InitiallyFocused 
        {
            private get; set;
        }

        public string Tooltip { get; set; }

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
                OnClick,
                onHoveredAction: OnHover,
                onUpAction: g => g.SetCursor = CursorType.Hand,
                onDownAction: g => g.SetCursor = CursorType.Hand
                );
        }

        protected virtual void OnHover(Gesture gesture)
        {
            gesture.SetCursor = CursorType.Hand;
            if(FocusOnOver)
            {
                Parent.Window.CurrentFocus = this;
            }
        }

        protected virtual void OnClick()
        {
            Command?.Execute(CommandParameter);
        }

        protected override void OnUpdate(float time)
        {
            if(InitiallyFocused)
            {
                InitiallyFocused = false;
                Window.CurrentFocus = this;
            }
            base.OnUpdate(time);

            if (CurrentState == ButtonState.Hover)
            {
                Services.TooltipService.ShowTooltip(this, Tooltip, ScreenBounds.BottomLeft);
            }
            else
            {
                Services.TooltipService.HideTooltip(this);
            }
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            Color foregroundColor = ForegroundColor;
            Color backgroundColor = BackgroundColor;

            var state = CurrentState;

            if(Parent.Window.CurrentFocus == this)
            {
                state = ButtonState.Hover;
            }

            switch (state)
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

        protected virtual void RenderButton(Canvas canvas, Color foregroundColor, Color backgroundColor, float opacity)
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

                case nameof(CommandParameter):
                    Command_CanExecuteChanged(this, EventArgs.Empty);
                    break;

                case nameof(BackgroundDrawable):
                    break;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs _)
        {
            Enabled = command?.CanExecute(CommandParameter) ?? true;
        }

        public bool HandleUiKey(UiInputKey key)
        {
            if (!Visible || !Enabled) return false;

            switch (key)
            {
                case UiInputKey.Select:
                    OnClick();
                    return true;
            }
            return Window.NavigateFocus(key);
        }

        protected override void Dispose(bool disposing)
        {
            if (Window.CurrentFocus == this)
            {
                Window.CurrentFocus = null;
            }

            base.Dispose(disposing);
        }

        public bool ResignFocus() => true;
    }
}
