using CrossX.Forms.Values;
using System;

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

        public bool IsFocused { get => isFocused; private set => SetProperty(ref isFocused, value); }
        private bool isFocused;

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
                        return true;
                    }
                    break;

                case UiButton.Right:
                    if (moveRight != null)
                    {
                        Focus = moveRight;
                        return true;
                    }
                    break;

                case UiButton.Down:
                    if (moveDown != null)
                    {
                        Focus = moveDown;
                        return true;
                    }
                    break;

                case UiButton.Up:
                    if (moveUp != null)
                    {
                        Focus = moveUp;
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

        public override void Update(TimeSpan frameTime)
        {
            base.Update(frameTime);
            IsFocused = (Focus == this);
        }
    }
}
