using CrossX.Framework.Graphics;
using System;

namespace CrossX.Framework
{
    public static class Utils
    {
        public static T Set<T>(this T obj, Action<T> action) where T: class
        {
            action(obj);
            return obj;
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
    }
}
