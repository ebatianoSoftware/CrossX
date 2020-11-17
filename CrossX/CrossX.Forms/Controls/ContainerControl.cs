using CrossX.Graphics2D;
using CrossX.IoC;
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
        public IObjectFactory ObjectFactory => Parent.ObjectFactory;

        protected readonly List<Control> children = new List<Control>();

        protected ContainerControl(IControlParent parent) : base(parent)
        {
        }

        public virtual void AddChild(Control control)
        {
            children.Add(control);
        }

        protected override void OnDraw(TimeSpan frameTime)
        {
            base.OnDraw(frameTime);

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

        public override void Dispose()
        {
            base.Dispose();
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].Dispose();
            }
        }

        public override void RecreateBindings()
        {
            BindingService.RecreateValues();

            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].RecreateBindings();
            }
        }
    }
}
