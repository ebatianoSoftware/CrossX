﻿using CrossX.Framework.Graphics;
using System;
using System.Numerics;

namespace CrossX.Framework.UI.Controls
{
    public class ImageView : View
    {
        private ImageDescriptor source;

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
                if(SetProperty(ref stretch, value))
                {
                    Services.RedrawService.RequestRedraw();
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

        private Image image;
        private float scale = 1;
        private Stretch stretch = Stretch.Uniform;

        public ImageView(IUIServices services): base(services)
        {
        }

        protected override void OnRender(Canvas canvas, float opacity)
        {
            base.OnRender(canvas, opacity);

            if (image == null) return;

            CalculateSourceAndTarget(out var target, out var source);
            canvas.DrawImage(image, target, source, opacity);
        }

        private void CalculateSourceAndTarget(out RectangleF target, out RectangleF source)
        {
            var width = image.Size.Width * Scale;
            var height = image.Size.Height * Scale;

            source = new Rectangle(0, 0, image.Size.Width, image.Size.Height);

            switch (Stretch)
            {
                case Stretch.None:
                    var pos = ScreenBounds.Center - new Vector2(width / 2, height / 2);
                    target = new RectangleF(pos, new SizeF(width, height));
                    return;

                case Stretch.Fill:
                    target = ScreenBounds;
                    return;

                case Stretch.Uniform:
                    var scale = Math.Min(ScreenBounds.Width / image.Size.Width, ScreenBounds.Height / image.Size.Height);
                    width = image.Size.Width * scale;
                    height = image.Size.Height * scale;
                    var pos2 = ScreenBounds.Center - new Vector2(width / 2, height / 2);
                    target = new RectangleF(pos2, new SizeF(width, height));
                    return;

                case Stretch.UniformToFill:
                    var scale2 = Math.Max(ScreenBounds.Width / image.Size.Width, ScreenBounds.Height / image.Size.Height);
                    width = image.Size.Width * scale2;
                    height = image.Size.Height * scale2;
                    target = ScreenBounds;

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
            
            if(desc.Uri == "")
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
                size.Width = image?.Size.Width * Scale ?? 0;
            }

            if (Height.IsAuto && VerticalAlignment != Alignment.Stretch)
            {
                size.Height = image?.Size.Height * Scale ?? 0;
            }
            return size;
        }
    }
}