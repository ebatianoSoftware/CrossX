using CrossX.Forms.Values;
using CrossX.Graphics2D.Text;
using CrossX.Input;
using System;
using System.Numerics;
using System.Windows.Input;

namespace CrossX.Forms.Controls
{
    public class Button : FocusablePanel
    {
        public TextSource Text { get => text; set => SetProperty(ref text, value); }
        public bool IsDown { get => isDown; private set => SetProperty(ref isDown, value); }
        public bool IsToggled { get => isToggled; set => SetProperty(ref isToggled, value); }
        public bool IsEnabled { get => isEnabled; private set => SetProperty(ref isEnabled, value); }
        public int PushAndExecuteTime { get => pushAndExecuteTime; set => SetProperty(ref pushAndExecuteTime, value); }

        public ICommand Command { get => command; set => SetProperty(ref command, value); }
        public object CommandParameter { get => commandParameter; set => SetProperty(ref commandParameter, value); }

        private TextSource text;

        private bool isEnabled = true;

        private bool isDown;

        private long? pointerId;
        private ICommand command;
        private object commandParameter;

        private ICommand oldCommand;

        private float timeToExecute = 0;
        private int pushAndExecuteTime = 100;
        private bool isToggled;

        private Matrix4x4 pushTransform;

        protected bool CommandEnabled { get; private set; } = true;

        public Button(IControlParent parent, IControlServices services) : base(parent, services)
        {
            Cursor = Input.CursorType.Hand;
        }

        protected override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);

            switch (name)
            {
                case nameof(Command):
                case nameof(CommandParameter):

                    if (oldCommand != null)
                    {
                        oldCommand.CanExecuteChanged -= Command_CanExecuteChanged;
                    }
                    oldCommand = Command;
                    CommandEnabled = Command?.CanExecute(CommandParameter) ?? true;
                    Command.CanExecuteChanged += Command_CanExecuteChanged;
                    break;
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            CommandEnabled = Command?.CanExecute(CommandParameter) ?? true;
        }

        protected override bool OnTouch(long id, TouchEvent evnt, Vector2 position)
        {
            if (!IsEnabled && !CommandEnabled || !IsVisible)
            {
                IsDown = false;
                pointerId = null;
            }

            switch (evnt)
            {
                case TouchEvent.Down:
                    if (CheckPointerIn(position) && !pointerId.HasValue && CommandEnabled)
                    {
                        Matrix4x4.Invert(CurrentTransform, out var inversion);
                        pushTransform = inversion;
                        pointerId = id;
                        IsDown = true;
                        Focus = this;
                        Services.Sounds.PushButton?.Play();
                        return true;
                    }
                    break;

                case TouchEvent.Up:
                    if (pointerId == id)
                    {
                        pointerId = null;
                        IsDown = false;

                        if (CheckPointerIn(position, pushTransform) && IsEnabled && CommandEnabled)
                        {
                            Toggle();
                            Services.Sounds.Select?.Play();
                            Command?.Execute(CommandParameter);
                        }
                    }
                    break;

                case TouchEvent.Idle:

                    if (CheckPointerIn(position))
                    {
                        Services.CursorType = CursorType.Hand;
                        return true;
                    }

                    if(IsDown && CheckPointerIn(position, pushTransform))
                    {
                        Services.CursorType = CursorType.Hand;
                        return true;
                    }
                    break;
                    

                case TouchEvent.Move:

                    if (pointerId == id)
                    {
                        IsDown = CheckPointerIn(position, pushTransform) && CommandEnabled;
                    }

                    if (IsDown || CheckPointerIn(position))
                    {
                        Services.CursorType = CursorType.Hand;
                        return true;
                    }

                    break;

                case TouchEvent.Remove:
                    if (pointerId == id)
                    {
                        pointerId = null;
                        IsDown = false;
                    }
                    break;
            }

            return base.OnTouch(id, evnt, position);
        }

        public override void OnPointerCaptured(long id, object capturedBy)
        {
            if (capturedBy == this) return;

            if (id == pointerId)
            {
                pointerId = null;
                IsDown = false;
            }
        }

        private bool CheckPointerIn(Vector2 position, Matrix4x4 transform)
        {
            position = Vector2.Transform(position, transform);

            var area = ClientArea;
            return area.Contains(new System.Drawing.PointF(position.X, position.Y));
        }

        public override bool OnUiButtonPressed(UiButton button)
        {
            if (button == UiButton.Select)
            {
                if (IsEnabled && CommandEnabled)
                {
                    timeToExecute = Math.Max(0.01f, (float)pushAndExecuteTime / 1000.0f);
                    Services.Sounds.PushButton?.Play();
                    IsDown = true;
                    return true;
                }
            }

            return base.OnUiButtonPressed(button);
        }

        protected void Toggle()
        {
            if (!BindingService.Contains(nameof(IsToggled)))
            {
                IsToggled = !IsToggled;
                return;
            }

            BindingService.TrySetValue(nameof(IsToggled), !IsToggled);
        }

        protected override void OnUpdate(TimeSpan frameTime)
        {
            base.OnUpdate(frameTime);

            if (timeToExecute > 0)
            {
                timeToExecute -= (float)frameTime.TotalSeconds;
                if (timeToExecute <= 0)
                {
                    Toggle();
                    if (CommandEnabled)
                    {
                        Services.Sounds.Select?.Play();
                        Command?.Execute(CommandParameter);
                    }
                    timeToExecute = 0;
                    IsDown = false;
                }
            }
        }
    }
}
