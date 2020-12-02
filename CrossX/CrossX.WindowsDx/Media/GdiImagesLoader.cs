using CrossX.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CrossX.WindowsDx.Media
{
    public class GdiImagesLoader : IImageLoader
    {
        public RawImage FromStream(Stream stream, bool premultiply)
        {
            Bitmap bm = (Bitmap)Image.FromStream(stream);
            var bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] bytes = new byte[bm.Width * bm.Height * 4];
            Marshal.Copy(bd.Scan0, bytes, 0, bytes.Length);
            bm.UnlockBits(bd);

            
            for(var idx =0; idx < bytes.Length; idx+=4)
            {
                int alpha = premultiply ? bytes[idx + 3] : 255;
                var a = bytes[idx + 3];
                var r = (byte)(bytes[idx + 2] * alpha / 255);
                var g = (byte)(bytes[idx + 1] * alpha / 255);
                var b = (byte)(bytes[idx + 0] * alpha / 255);

                bytes[idx + 0] = r;
                bytes[idx + 1] = g;
                bytes[idx + 2] = b;
                bytes[idx + 3] = a;
            }
            return new RawImage(bm.Width, bm.Height, bytes, PixelDataFormat.Format32bppRGBA | (premultiply ? PixelDataFormat.Premultiplied : 0));
        }

        public RawImage FromStream(Stream stream)
        {
            return FromStream(stream, false);
        }
    }
}
