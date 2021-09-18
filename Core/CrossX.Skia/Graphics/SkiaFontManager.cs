using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace CrossX.Skia.Graphics
{
    internal class SkiaFontManager : IFontManager
    {
        struct FontDesc : IEquatable<FontDesc>
        {
            public string FamilyName;
            public int Size;
            public FontWeight Weight;
            public bool Italic;

            public override bool Equals(object obj)
            {
                return obj is FontDesc desc && Equals(desc);
            }

            public bool Equals(FontDesc other)
            {
                return FamilyName == other.FamilyName &&
                       Size == other.Size &&
                       Weight == other.Weight &&
                       Italic == other.Italic;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(FamilyName, Size, Weight, Italic);
            }
        }

        private Dictionary<FontDesc, SkiaFont> fonts = new Dictionary<FontDesc, SkiaFont>();

        public Font FindFont(string familyName, float fontSize, FontWeight fontWeight, bool italic)
        {
            var fd = new FontDesc
            {
                FamilyName = familyName,
                Size = (int)(fontSize * 10),
                Weight = fontWeight,
                Italic = italic
            };

            if(!fonts.TryGetValue(fd, out var font))
            {
                var typeface = SKTypeface.FromFamilyName(familyName, fontWeight.ToSkia(), SKFontStyleWidth.Normal, italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
                font = new SkiaFont(typeface.ToFont(fontSize));
                fonts.Add(fd, font);
            }

            return font;
        }
    }
}
