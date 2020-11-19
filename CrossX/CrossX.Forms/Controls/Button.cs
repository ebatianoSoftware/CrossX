using CrossX.Forms.Values;
using CrossX.Graphics2D.Text;
using System;
using System.Windows.Input;

namespace CrossX.Forms.Controls
{
    public class Button : FocusablePanel
    {
        public TextSource Text { get => text; set => SetProperty(ref text, value); }
        public bool IsDown { get => isDown; private set => SetProperty(ref isDown, value); }
        
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

        protected bool CommandEnabled { get; private set; } = true;

        public Button(IControlParent parent) : base(parent)
        {
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
            if (base.OnTouch(id, evnt, position)) return true;

            if (!IsEnabled && !CommandEnabled)
            {
                IsDown = false;
                pointerId = null;
            }

            switch (evnt)
            {
                case TouchEvent.Down:
                    if (CheckPointerIn(position) && !pointerId.HasValue && CommandEnabled)
                    {
                        pointerId = id;
                        IsDown = true;
                        Focus = this;
                        return true;
                    }
                    break;

                case TouchEvent.Up:
                    if (pointerId == id)
                    {
                        pointerId = null;
                        IsDown = false;

                        if (CheckPointerIn(position) && IsEnabled && CommandEnabled)
                        {
                            Command?.Execute(CommandParameter);
                        }
                    }
                    break;

                case TouchEvent.Move:
                    if (pointerId == id)
                    {
                        IsDown = CheckPointerIn(position) && CommandEnabled;
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

        private bool CheckPointerIn(Vector2 position)
        {
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
                    IsDown = true;
                    return true;
                }
            }

            return base.OnUiButtonPressed(button);
        }

        public override void Update(TimeSpan frameTime)
        {
            base.Update(frameTime);
            if(timeToExecute > 0)
            {
                timeToExecute -= (float)frameTime.TotalSeconds;
                if(timeToExecute <= 0)
                {
                    if (CommandEnabled)
                    {
                        Command?.Execute(CommandParameter);
                    }
                    timeToExecute = 0;
                    IsDown = false;
                }
            }
        }
    }
}
