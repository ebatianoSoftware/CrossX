using CrossX.Forms.Values;
using CrossX.Graphics2D;
using System;
using System.Drawing;
using System.Numerics;

namespace CrossX.Forms.Controls
{
    public class Image : Control
    {
        private ImageSource source;
        private Stretch stretch;
        private SpriteFlags flags;

        public ImageSource Source { get => source; set => SetProperty(ref source, value); }
        public Stretch Stretch { get => stretch; set => SetProperty(ref stretch, value); }
        public SpriteFlags Flags { get => flags; set => SetProperty(ref flags, value); }

        public Image(IControlParent parent, IControlServices services) : base(parent, services)
        {
            HorizontalAlignment = Alignment.Start;
            VerticalAlignment = Alignment.Start;
            Stretch = Stretch.Uniform;
        }

        protected override void OnDraw(TimeSpan frameTime, Color4 tintColor)
        {
            base.OnDraw(frameTime, tintColor);
            if (Source.Texture == null) return;

            var rect = Source.SourceRect;
            var position = new Vector2(ActualX, ActualY);
            var scale = Vector2.One;

            switch (stretch)
            {
                case Stretch.Uniform:
                    scale.X = scale.Y = Math.Min(ActualWidth / rect.Width, ActualHeight / rect.Height);
                    break;

                case Stretch.UniformToFill:
                    scale.X = scale.Y = Math.Max(ActualWidth / rect.Width, ActualHeight / rect.Height);
                    break;

                case Stretch.Fill:
                    scale.X = ActualWidth / rect.Width;
                    scale.Y = ActualHeight / rect.Height;
                    break;
            }

            position.X += (ActualWidth - rect.Width * scale.X) / 2;
            position.Y += (ActualHeight - rect.Height * scale.Y) / 2;

            var targetWidth = rect.Width * scale.X;
            var targetHeight = rect.Height * scale.Y;

            if (targetWidth > ActualWidth)
            {
                position.X = ActualX;
                var diff = (targetWidth - ActualWidth) / scale.X;

                rect.X += (int)Math.Floor(diff / 2);
                rect.Width -= (int)Math.Ceiling(diff);
            }

            if (targetHeight > ActualHeight)
            {
                position.Y = ActualY;
                var diff = (targetHeight - ActualHeight) / scale.Y;

                rect.Y += (int)Math.Floor(diff / 2);
                rect.Height -= (int)Math.Ceiling(diff);
            }

            Services.SpriteBatch.DrawImage(Source.Texture, position, rect, tintColor, scale, flags);
        }

        public override Vector2 CalculateSize(RectangleF clientArea, bool includeMargins)
        {
            var size = base.CalculateSize(clientArea, includeMargins);

            if (Width.IsAuto && HorizontalAlignment != Alignment.Stretch)
            {
                if (!Height.IsAuto && source.SourceRect.Height > 0)
                {
                    size.X = source.SourceRect.Width * size.Y / source.SourceRect.Height;
                }
                else
                {
                    size.X = source.SourceRect.Width;
                }
            }

            if (Height.IsAuto && VerticalAlignment != Alignment.Stretch)
            {
                if (!Width.IsAuto && source.SourceRect.Width > 0)
                {
                    size.Y = source.SourceRect.Height * size.X / source.SourceRect.Width;
                }
                else
                {
                    size.Y = source.SourceRect.Height;
                }
            }

            return size;
        }
    }
}
