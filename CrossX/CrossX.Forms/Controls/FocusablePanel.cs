using CrossX.Forms.Values;
using System;
using System.Windows.Input;

namespace CrossX.Forms.Controls
{
    public class FocusablePanel : Panel, IFocusable
    {
        private IFocusable moveLeft;
        private IFocusable moveRight;
        private IFocusable moveUp;
        private IFocusable moveDown;

        public IFocusable MoveLeftTo { get => moveLeft; set => SetProperty(ref moveLeft, value); }
        public IFocusable MoveRightTo { get => moveRight; set => SetProperty(ref moveRight, value); }
        public IFocusable MoveUpTo { get => moveUp; set => SetProperty(ref moveUp, value); }
        public IFocusable MoveDownTo { get => moveDown; set => SetProperty(ref moveDown, value); }

        public ICommand LeftCommand { get => leftCommand; set => SetProperty(ref leftCommand, value); }
        public ICommand RightCommand { get => rightCommand; set => SetProperty(ref rightCommand, value); }

        public bool IsFocused { get => isFocused; private set => SetProperty(ref isFocused, value); }
        private bool isFocused;
        private ICommand leftCommand;
        private ICommand rightCommand;

        public FocusablePanel(IControlParent parent, IControlServices services) : base(parent, services)
        {
        }

        public virtual bool OnUiButtonPressed(UiButton button)
        {
            switch (button)
            {
                case UiButton.Left:
                    if (moveLeft != null)
                    {
                        Focus = moveLeft;
                        Services.Sounds.Focus?.Play();
                        return true;
                    }
                    break;

                case UiButton.Right:
                    if (moveRight != null)
                    {
                        Focus = moveRight;
                        Services.Sounds.Focus?.Play();
                        return true;
                    }
                    break;

                case UiButton.Down:
                    if (moveDown != null)
                    {
                        Focus = moveDown;
                        Services.Sounds.Focus?.Play();
                        return true;
                    }
                    break;

                case UiButton.Up:
                    if (moveUp != null)
                    {
                        Focus = moveUp;
                        Services.Sounds.Focus?.Play();
                        return true;
                    }
                    break;
            }

            return false;
        }

        public bool OnUiButtonReleased(UiButton button)
        {
            return false;
        }

        public bool OnCharacterInput(char character)
        {
            return false;
        }

        protected override void OnUpdate(TimeSpan frameTime)
        {
            base.OnUpdate(frameTime);
            IsFocused = (Focus == this);
        }
    }
}
