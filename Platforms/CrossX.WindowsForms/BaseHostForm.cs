using CrossX.Framework.Input;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrossX.WindowsForms
{
    public abstract class BaseHostForm: Form
    {
        private WindowHost currentModalChild;
        private WindowHost currentChild;
        private Point childRelativePosition;

        protected abstract CursorType CursorType { set; }

        protected abstract bool EnableManipulation { set; }

        public BaseHostForm()
        {
            LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object sender, EventArgs args)
        {
            if (currentChild != null)
            {
                currentChild.Location = new Point(Location.X + childRelativePosition.X, Location.Y + childRelativePosition.Y);
            }
        }

        public void AddModal(WindowHost host)
        {
            currentModalChild = host;
            CursorType = CursorType.Default;
            EnableManipulation = false;

            AddChild(host);
        }

        public void AddChild(WindowHost host)
        {
            currentChild = host;
            currentChild.Disposed += CurrentChild_Disposed;

            host.StartPosition = FormStartPosition.Manual;
            currentChild.Shown += CurrentChild_Shown;

            AddOwnedForm(host);
        }

        private void CurrentChild_Shown(object sender, EventArgs e)
        {
            var offset = PointToScreen(Point.Empty);
            offset.X -= Location.X;
            offset.Y -= Location.Y;

            offset.X += (ClientSize.Width - currentChild.Size.Width) / 2;
            offset.Y += (ClientSize.Height - currentChild.Size.Height) / 2;

            childRelativePosition = offset;
            currentChild.Shown -= CurrentChild_Shown;

            currentChild.LocationChanged += CurrentChild_LocationChanged;
            OnLocationChanged(this, EventArgs.Empty);
        }

        private void CurrentChild_Disposed(object sender, EventArgs e)
        {
            currentChild.Disposed -= CurrentChild_Disposed;
            currentChild.LocationChanged -= CurrentChild_LocationChanged;
            RemoveOwnedForm(currentChild);
            currentChild = null;
            currentModalChild = null;
            EnableManipulation = true;
        }

        private void CurrentChild_LocationChanged(object sender, EventArgs args)
        {
            childRelativePosition = new Point(currentChild.Location.X - Location.X, currentChild.Location.Y - Location.Y);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (currentModalChild != null)
            {
                currentModalChild?.Activate();
            }
        }

    }
}
