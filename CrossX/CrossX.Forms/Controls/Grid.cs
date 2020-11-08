using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrossX.Forms.Controls
{
    public enum GridLengthMode
    {
        Value,
        Star,
        Auto
    }
    public struct GridLength
    {
        public GridLengthMode Mode { get; }
        public float Value { get; }

        public GridLength(GridLengthMode mode, float value)
        {
            Mode = mode;
            Value = value;
        }
    }

    public class Grid : Control
    {
        public GridLength[] ColumnDefinitions { get; set; }
        public GridLength[] RowDefinitions { get; set; }
        public Color4 Background { get => background; set => SetProperty(ref background, value); }

        public float[] ActualColumnWidth { get; }
        public float[] ActualColumnHeight { get; }

        public IEnumerable<Control> Children => children;
        private List<Control> children = new List<Control>();
        private Color4 background;

        public Grid(IControlParent parent) : base(parent)
        {
        }

        protected override void CalculateLayout()
        {
            base.CalculateLayout();

            for (var idx = 0; idx < children.Count; ++idx)
            {
                var size = children[idx].CalculateSize(ClientArea);
                var position = children[idx].CalculatePosition(ClientArea, size);
                children[idx].PositionControl(position, size);
            }
        }

        public override void AddChild(Control control)
        {
            children.Add(control);
        }

        public override void Draw(TimeSpan frameTime)
        {
            if(background.A > 0)
            {
                Parent.PrimitiveBatch.DrawRect(new RectangleF(ActualX, ActualY, ActualWidth, ActualHeight), background);
            }

            for(var idx =0; idx < children.Count; ++idx)
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
    }
}
