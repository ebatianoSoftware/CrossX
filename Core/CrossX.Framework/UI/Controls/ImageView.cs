using CrossX.Framework.Graphics;
using System;
using System.Numerics;

namespace CrossX.Framework.UI.Controls
{
    public class ImageView : View
    {
        private ImageDescriptor source;
        private Thickness padding;

        public ImageDescriptor Source
        {
            get => source;

            set
            {
                if (SetProperty(ref source, value))
                {
                    SetImage(value);
                }
            }
        }

        public Stretch Stretch
        {
            get => stretch;
            set
            {
                if (SetProperty(ref stretch, value))
                {
                    Invalidate();
                }
            }
        }

        public float Scale
        {
            get => scale;
            set
            {
                if (SetProperty(ref scale, value))
                {
                    Parent?.InvalidateLayout();
                }
            }
        }

        public Thickness Padding
        {
            get => padding;
            set
            {
                if (SetProperty(ref padding, value))
                {
                    Parent?.InvalidateLayout();
                }
            }
        }

        public bool FlipHorizontal { get => flipHorizontal; set => SetPropertyAndRedraw(ref flipHorizontal, value); }

        private Image image;
        private float scale = 1;
        private Stretch stretch = Stretch.Uniform;
        private bool flipHorizontal;

        public ImageView(IUIServices services) : base(services)
        {
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);

            if (image == null) return;

            CalculateSourceAndTarget(out var target, out var source);

            if (FlipHorizontal)
            {
                canvas.SaveState();
                canvas.Transform(Matrix3x2.CreateScale(-1, 1, target.Center));
                canvas.DrawImage(image, target, source, opacity);
                canvas.Restore();
            }
            else
            {
                canvas.DrawImage(image, target, source, opacity);
            }
        }

        private void CalculateSourceAndTarget(out RectangleF target, out RectangleF source)
        {
            var width = image.Size.Width * Scale;
            var height = image.Size.Height * Scale;

            source = new Rectangle(0, 0, image.Size.Width, image.Size.Height);

            var screenBounds = ScreenBounds.Deflate(Padding);

            switch (Stretch)
            {
                case Stretch.None:
                    var pos = screenBounds.Center - new Vector2(width / 2, height / 2);
                    target = new RectangleF(pos, new SizeF(width, height));
                    return;

                case Stretch.Fill:
                    target = screenBounds;
                    return;

                case Stretch.Uniform:
                    var scale = Math.Min(screenBounds.Width / image.Size.Width, screenBounds.Height / image.Size.Height);
                    width = image.Size.Width * scale;
                    height = image.Size.Height * scale;
                    var pos2 = screenBounds.Center - new Vector2(width / 2, height / 2);
                    target = new RectangleF(pos2, new SizeF(width, height));
                    return;

                case Stretch.UniformToFill:
                    var scale2 = Math.Max(screenBounds.Width / image.Size.Width, screenBounds.Height / image.Size.Height);
                    width = image.Size.Width * scale2;
                    height = image.Size.Height * scale2;
                    target = screenBounds;

                    var cutW = (width - target.Width) / scale2;
                    var cutH = (height - target.Height) / scale2;

                    source = new RectangleF(cutW / 2, cutH / 2, image.Size.Width - cutW, image.Size.Height - cutH);
                    return;
            }
            throw new ArgumentOutOfRangeException(nameof(Stretch));
        }

        private async void SetImage(ImageDescriptor desc)
        {
            if (desc.Image != null)
            {
                image = desc.Image;
                Parent?.InvalidateLayout();
                return;
            }

            if (desc.Uri == "")
            {
                image = null;
            }

            try
            {
                var img = await Services.ImageCache.GetImage(desc.Uri);
                Services.Dispatcher.BeginInvoke(() =>
                {
                    if (Source.Uri == desc.Uri)
                    {
                        Source = new ImageDescriptor(desc.Uri, img);
                    }
                    else
                    {
                        image = img;
                        Parent?.InvalidateLayout();
                    }
                });
            }
            catch
            {
            }
        }

        public override SizeF CalculateSize(SizeF parentSize)
        {
            var size = base.CalculateSize(parentSize);

            if (Width.IsAuto && HorizontalAlignment != Alignment.Stretch)
            {
                size.Width = image?.Size.Width * Scale ?? 0 + Padding.Width;
            }

            if (Height.IsAuto && VerticalAlignment != Alignment.Stretch)
            {
                size.Height = image?.Size.Height * Scale ?? 0 + Padding.Height;
            }

            return new SizeF(size.Width, size.Height);
        }
    }
}
