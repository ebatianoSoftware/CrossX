using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System;
using System.IO;

namespace CrossX.Skia.Graphics
{
    internal class SkiaImage : Image
    {
        public SKImage SKImage { get; }

        public SKShader SKShader
        {
            get
            {
                if(skShader == null || skShader.Handle == IntPtr.Zero)
                {
                    skShader = SKShader.CreateImage(SKImage);
                }
                return skShader;
            }
        }

        private SKShader skShader;

        public override Size Size => new Size(SKImage.Width, SKImage.Height);

        public SkiaImage(Stream stream)
        {
            SKImage = SKImage.FromEncodedData(stream);
        }

        public SkiaImage(SKImage img)
        {
            SKImage = img;
        }

        protected override void Dispose(bool disposing)
        {
            if (skShader != null && skShader.Handle != IntPtr.Zero)
            {
                skShader.Dispose();
            }
            SKImage.Dispose();
        }

        public override Image Scale(float scale)
        {
            using (var oldBitmap = SKBitmap.FromImage(SKImage))
            {
                SKImageInfo resizeInfo = new SKImageInfo( (int)(SKImage.Width * scale), (int)(SKImage.Height * scale));
                using (var resizedSKBitmap = oldBitmap.Resize(resizeInfo, SKFilterQuality.High))
                {
                    return new SkiaImage(SKImage.FromBitmap(resizedSKBitmap));
                }
            }
        }
    }
}
