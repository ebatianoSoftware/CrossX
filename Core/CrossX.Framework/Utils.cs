using CrossX.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;

namespace CrossX.Framework
{
    public static class Utils
    {
        public static T Set<T>(this T obj, Action<T> action) where T: class
        {
            action(obj);
            return obj;
        }

        public static Rectangle GetPixelsRect(this RectangleF rect)
        {
            return new Rectangle((int)(rect.X * UiUnit.PixelsPerUnit), (int)(rect.Y * UiUnit.PixelsPerUnit),
                (int)(rect.Width * UiUnit.PixelsPerUnit), (int)(rect.Height * UiUnit.PixelsPerUnit));
        }

        public static TextAlign GetTextAlign(Alignment horzAlign, Alignment vertAlign)
        {
            TextAlign align = TextAlign.Left;

            switch(horzAlign)
            {
                case Alignment.Center:
                    align |= TextAlign.Center;
                    break;

                case Alignment.End:
                    align |= TextAlign.Right;
                    break;
            }

            switch (vertAlign)
            {
                case Alignment.Center:
                    align |= TextAlign.Middle;
                    break;

                case Alignment.End:
                    align |= TextAlign.Bottom;
                    break;
            }
            return align;
        }

        public static Alignment Oposite(this Alignment align)
        {
            switch(align)
            {
                case Alignment.Start:
                    return Alignment.End;

                case Alignment.Center:
                    return Alignment.Center;

                case Alignment.End:
                    return Alignment.Start;
            }
            return Alignment.Stretch;
        }

        public static Stream OpenEmbededResource(string uri)
        {
            var parts = uri.Split(';');
            if (parts.Length == 2)
            {
                var assembly = Assembly.Load(parts[1]);
                var stream = assembly.GetManifestResourceStream(parts[0]);

                if (stream == null) throw new FileNotFoundException();
                return stream;
            }
            throw new ArgumentException("Invalid uri for embeded resource", nameof(uri));
        }
    }
}
