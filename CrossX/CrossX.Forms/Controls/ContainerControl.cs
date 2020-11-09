using CrossX.Graphics2D;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public abstract class ContainerControl : Control, IControlParent
    {
        public Color4 Background { get => background; set => SetProperty(ref background, value); }
        private Color4 background;

        public IEnumerable<Control> Children => children;

        public SpriteBatch SpriteBatch => Parent.SpriteBatch;
        public PrimitiveBatch PrimitiveBatch => Parent.PrimitiveBatch;
        public IControlsLoader ControlsLoader => Parent.ControlsLoader;
        protected readonly List<Control> children = new List<Control>();

        protected ContainerControl(IControlParent parent) : base(parent)
        {
        }

        public virtual void AddChild(Control control)
        {
            children.Add(control);
        }

        public override void Draw(TimeSpan frameTime)
        {
            if (background.A > 0)
            {
                Parent.PrimitiveBatch.DrawRect(new RectangleF(ActualX, ActualY, ActualWidth, ActualHeight), background);
            }

            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].Draw(frameTime);
            }
        }

        public override void Update(TimeSpan frameTime)
        {
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].Update(frameTime);
            }
        }

        public override void BeforeUpdate()
        {
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].BeforeUpdate();
            }
        }
    }
}
