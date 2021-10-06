using System;
using System.Collections.Generic;
using System.Numerics;

namespace CrossX.Framework.UI.Containers
{
    public class WrapLayout : ViewContainer
    {
        private Length spacing;
        private Orientation orientation = Orientation.Vertical;
        private Alignment lineVerticalAlignment = Alignment.Center;
        private Alignment lineHorizontalAlignment = Alignment.Center;

        private List<float> lineHeights = new List<float>();
        private List<float> lineWidths = new List<float>();
        private List<int> lineIndices = new List<int>();
        private List<RectangleF> childPositions = new List<RectangleF>();
        

        public Orientation Orientation
        {
            get => orientation;
            set => SetPropertyAndRecalcLayout(ref orientation, value);
        }

        public Length Spacing
        {
            get => spacing;
            set => SetPropertyAndRecalcLayout(ref spacing, value);
        }

        public Alignment LineHorizontalAlignment { get => lineHorizontalAlignment; set => SetPropertyAndRecalcLayout(ref lineHorizontalAlignment, value); }
        public Alignment LineVerticalAlignment { get => lineVerticalAlignment; set => SetPropertyAndRecalcLayout(ref lineVerticalAlignment, value); }

        public WrapLayout(IUIServices services) : base(services)
        {
        }

        protected override void RecalculateLayout()
        {
            base.RecalculateLayout();

            if (Bounds.Size.Width == 0) return;
            if (Bounds.Size.Height == 0) return;

            if (childPositions.Count == 0)
            {
                lineHeights.Clear();
                lineWidths.Clear();
                lineIndices.Clear();
                childPositions.Clear();

                if (Orientation == Orientation.Horizontal)
                {
                    var size = PositionHorizontal(Bounds.Size);
                    if (Bounds.Size != size) Parent?.InvalidateLayout();
                }
                else
                {
                    var size = PositionVertical(Bounds.Size);
                    if (Bounds.Size != size) Parent?.InvalidateLayout();
                }
            }

            var index = 0;
            for (var idx =0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                if(child.DisplayVisible)
                {
                    child.Bounds = childPositions[index++];
                }
            }
            
            lineHeights.Clear();
            lineWidths.Clear();
            lineIndices.Clear();
            childPositions.Clear();
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            lineHeights.Clear();
            lineWidths.Clear();
            lineIndices.Clear();
            childPositions.Clear();

            if (Orientation == Orientation.Horizontal)
            {
                if (size.Width == 0) return size;
                size = PositionHorizontal(size);
            }
            else
            {
                if (size.Height == 0) return size;
                size = PositionVertical(size);
            }
            return size;
        }

        private SizeF PositionVertical(SizeF size)
        {
            float currentHeight = 0;
            float maxHeight = size.Height - Padding.Height;

            float width = 0;
            float currentWidth = 0;

            var mySize = SizeF.Zero;

            var currentLine = 0;

            for (var idx = 0; idx < Children.Count;)
            {
                var child = Children[idx];
                if (child.DisplayVisible)
                {
                    var childSize = child.CalculateSize(mySize);
                    childSize.Height = Math.Min(maxHeight, childSize.Height);

                    var newHeight = currentHeight + childSize.Height + child.Margin.Height;

                    if (newHeight > maxHeight)
                    {
                        lineWidths.Add(currentWidth + Spacing.Calculate());
                        lineHeights.Add(currentHeight - Spacing.Calculate());

                        width += currentWidth + Spacing.Calculate();

                        currentHeight = 0;
                        currentWidth = 0;
                        currentLine++;
                        continue;
                    }

                    childPositions.Add(new RectangleF(width + child.Margin.Left.Calculate(), currentHeight + child.Margin.Top.Calculate(), childSize.Width, childSize.Height));
                    lineIndices.Add(currentLine);

                    currentWidth = Math.Max(currentWidth, childSize.Width);
                    currentHeight = newHeight + Spacing.Calculate();
                }
                ++idx;
            }

            lineHeights.Add(currentHeight - Spacing.Calculate());
            lineWidths.Add(currentWidth + Spacing.Calculate());

            int index = 0;
            var padding = new Vector2(Padding.Left.Calculate(), Padding.Top.Calculate());
            float maxWidth = 0;

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                if (child.DisplayVisible)
                {
                    var rect = childPositions[index];
                    int childLine = lineIndices[index];

                    var lineWidth = lineWidths[childLine];
                    var lineHeight = lineHeights[childLine];

                    var offset = Vector2.Zero;

                    switch (LineHorizontalAlignment)
                    {
                        case Alignment.Center:
                            offset.X = (lineWidth - rect.Width) / 2;
                            break;

                        case Alignment.End:
                            offset.X = (lineWidth - rect.Width);
                            break;
                    }

                    switch (LineVerticalAlignment)
                    {
                        case Alignment.Center:
                            offset.Y = (size.Height - Padding.Height - lineHeight) / 2;
                            break;

                        case Alignment.End:
                            offset.Y = (size.Height - Padding.Height - lineHeight);
                            break;
                    }

                    offset += padding;
                    childPositions[index] = rect.Offset(offset);
                    maxWidth = Math.Max(maxWidth, childPositions[index].Right);
                    ++index;
                }
            }

            return new SizeF(maxWidth + Padding.Right.Calculate(), size.Height);
        }

        private SizeF PositionHorizontal(SizeF size)
        {
            float currentWidth = 0;
            float maxWidth = size.Width - Padding.Width;

            float height = 0;
            float currentHeight = 0;

            var mySize = SizeF.Zero;

            var currentLine = 0;

            for (var idx = 0; idx < Children.Count;)
            {
                var child = Children[idx];
                if (child.DisplayVisible)
                {
                    var childSize = child.CalculateSize(mySize);
                    childSize.Width = Math.Min(maxWidth, childSize.Width);

                    var newWidth = currentWidth + childSize.Width + child.Margin.Width;

                    if (newWidth > maxWidth)
                    {
                        lineHeights.Add(currentHeight + Spacing.Calculate());
                        lineWidths.Add(currentWidth - Spacing.Calculate());

                        height += currentHeight + Spacing.Calculate();

                        currentHeight = 0;
                        currentWidth = 0;
                        currentLine++;
                        continue;
                    }

                    childPositions.Add(new RectangleF(currentWidth + child.Margin.Left.Calculate(), height + child.Margin.Top.Calculate(), childSize.Width, childSize.Height));
                    lineIndices.Add(currentLine);

                    currentHeight = Math.Max(currentHeight, childSize.Height);
                    currentWidth = newWidth + Spacing.Calculate();
                }
                ++idx;
            }

            lineWidths.Add(currentWidth - Spacing.Calculate());
            lineHeights.Add(currentHeight + Spacing.Calculate());

            int index = 0;
            var padding = new Vector2(Padding.Left.Calculate(), Padding.Top.Calculate());
            float maxHeight = 0;

            for (var idx = 0; idx < Children.Count; ++idx)
            {
                var child = Children[idx];
                if (child.DisplayVisible)
                {
                    var rect = childPositions[index];
                    int childLine = lineIndices[index];

                    var lineWidth = lineWidths[childLine];
                    var lineHeight = lineHeights[childLine];

                    var offset = Vector2.Zero;

                    switch(LineHorizontalAlignment)
                    {
                        case Alignment.Center:
                            offset.X = (size.Width - Padding.Width - lineWidth) / 2;
                            break;

                        case Alignment.End:
                            offset.X = (size.Width - Padding.Width - lineWidth);
                            break;
                    }

                    switch (LineVerticalAlignment)
                    {
                        case Alignment.Center:
                            offset.Y = (lineHeight - rect.Height) / 2;
                            break;

                        case Alignment.End:
                            offset.Y = (lineHeight - rect.Height);
                            break;
                    }

                    offset += padding;
                    childPositions[index] = rect.Offset(offset);
                    maxHeight = Math.Max(maxHeight, childPositions[index].Bottom);
                    ++index;
                }
            }

            return new SizeF(size.Width, maxHeight + Padding.Bottom.Calculate());
        }

        public override void InvalidateLayout()
        {
            Parent?.InvalidateLayout();
            base.InvalidateLayout();
        }

    }
}
