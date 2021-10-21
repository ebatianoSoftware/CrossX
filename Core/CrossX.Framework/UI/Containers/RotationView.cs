using CrossX.Abstractions.Input;
using CrossX.Framework.Graphics;
using CrossX.Framework.Input;
using CrossX.Framework.UI.Global;
using CrossX.Framework.XxTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xx;

namespace CrossX.Framework.UI.Containers
{
    [XxSchemaExport(XxChildrenMode.OnlyOne)]
    public class RotationView : View, IElementsContainer, IViewParent
    {
        private View currentView = null;
        private SizeF calculatedChildSize;

        private Matrix3x2 currentTransform = Matrix3x2.Identity;
        private Matrix3x2 currentTransformInv = Matrix3x2.Identity;
        
        private float rotation;
        public float Rotation 
        { 
            get => rotation;
            set
            {
                if (SetPropertyAndRedraw(ref rotation, value))
                {
                    UpdateTransforms();
                }
            }
        }

        public float RotationSpeed { get; set; }


        public RotationView(IUIServices services) : base(services)
        {

        }

        public Window Window => Parent?.Window;

        public void InitChildren(IEnumerable<object> elements)
        {
            currentView = elements.First() as View;
            currentView.Parent = this;

            InvalidateLayout();
        }

        private void UpdateTransforms()
        {
            var rot = (float)(Math.PI * rotation / 180.0);

            currentTransform = Matrix3x2.CreateRotation(rot, ScreenBounds.Center);
            currentTransformInv = Matrix3x2.CreateRotation(-rot, ScreenBounds.Center);
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            if (currentView == null) return SizeF.Zero;

            calculatedChildSize = currentView.CalculateSize(parentSize);

            var wh = Math.Max(calculatedChildSize.Width, calculatedChildSize.Height);
            return new SizeF(wh, wh);
        }

        protected override void RecalculateLayout()
        {
            if (currentView == null) return;
            var pos = currentView.CalculatePosition(calculatedChildSize, Bounds.Size);
            currentView.Bounds = new RectangleF(pos, calculatedChildSize);

            UpdateTransforms();
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);
            canvas.SaveState();
            canvas.Transform(currentTransform);
            currentView?.Render(canvas, opacity);
            canvas.Restore();
        }

        protected override bool OnProcesssUiKey(UiInputKey key)
        {
            if (currentView?.ProcessUiKey(key) == true) return true;
            return false;
        }

        public override void GetFocusables(IList<IFocusable> list)
        {
            currentView?.GetFocusables(list);
            base.GetFocusables(list);
        }

        protected override void OnUpdate(float time)
        {
            base.OnUpdate(time);

            Rotation += RotationSpeed * time;

            currentView?.Update(time);
        }

        protected override bool OnPreviewGesture(Gesture gesture)
        {
            var oldPosition = gesture.Position;
            gesture.Position = Vector2.Transform(gesture.Position, currentTransformInv);

            var result = currentView?.PreviewGesture(gesture) ?? false;

            gesture.Position = oldPosition;
            return result;
        }

        protected override bool OnProcessGesture(Gesture gesture)
        {
            var oldPosition = gesture.Position;
            gesture.Position = Vector2.Transform(gesture.Position, currentTransformInv);

            var result = currentView?.ProcessGesture(gesture) ?? false;

            gesture.Position = oldPosition;
            return result;
        }

        public void InvalidateLayout() => Parent?.InvalidateLayout();
    }
}
