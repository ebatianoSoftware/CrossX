using CrossX.Forms.Values;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CrossX.Forms.Controls
{
    public abstract class ContainerControl : Control, IControlParent
    {
        public IEnumerable<Control> Children => children;

        public override bool TransitionInProgress
        {
            get
            {
                if (Transition != null) return true;

                for(var idx=0; idx < children.Count; ++idx)
                {
                    if (children[idx].TransitionInProgress) return true;
                }

                return false;
            }
        }

        public IFocusable Focus
        { 
            get => Parent.Focus;
            set
            {
                if (Parent.Focus != value)
                {
                    FirePropertyChanging();
                    Parent.Focus = value;
                    FirePropertyChanged();
                }
            }
        }

        protected readonly List<Control> children = new List<Control>();

        protected ContainerControl(IControlParent parent, IControlServices services) : base(parent, services)
        {
        }

        public virtual void AddChild(Control control)
        {
            children.Add(control);
        }

        protected override void OnDraw(TimeSpan frameTime, Color4 tintColor)
        {
            base.OnDraw(frameTime, tintColor);

            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].Draw(frameTime, tintColor);
            }
        }

        protected override void OnUpdate(TimeSpan frameTime)
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

        protected override bool OnTouch(long id, TouchEvent evnt, Vector2 position)
        {
            for (var idx = children.Count-1; idx >= 0; --idx)
            {
                if (children[idx].ProcessTouch(id, evnt, position)) return true;
            }

            return base.OnTouch(id, evnt, position);
        }

        public override void OnPointerCaptured(long id, object capturedBy)
        {
            base.OnPointerCaptured(id, capturedBy);
            for (var idx = 0; idx < children.Count; ++idx)
            {
                children[idx].OnPointerCaptured(id, capturedBy);
            }
        }

        public override void TriggerEvent(string name)
        {
            base.TriggerEvent(name);

            for(var idx =0; idx < children.Count; ++idx)
            {
                children[idx].TriggerEvent(name);
            }
        }
    }
}
