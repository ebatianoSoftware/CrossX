using CrossX.Graphics2D;
using System;
using System.Collections.Generic;

namespace CrossX.Forms.Controls
{
    public abstract class ContainerControl : Control, IControlParent
    {
        

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
            base.Draw(frameTime);

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
            base.BeforeUpdate();
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].BeforeUpdate();
            }
        }
    }
}
