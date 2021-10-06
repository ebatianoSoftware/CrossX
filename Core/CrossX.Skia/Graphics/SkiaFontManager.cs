using CrossX.Framework;
using CrossX.Framework.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrossX.Skia.Graphics
{
    internal class SkiaFontManager : IFontManager
    {
        struct FontDesc : IEquatable<FontDesc>
        {
            public TypefaceDesc Typeface;
            public int Size;

            public override bool Equals(object obj)
            {
                return obj is FontDesc desc && Equals(desc);
            }

            public bool Equals(FontDesc other)
            {
                return Typeface.Equals(other.Typeface) &&
                       Size == other.Size;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Typeface, Size);
            }
        }

        struct TypefaceDesc: IEquatable<TypefaceDesc>
        {
            public string FamilyName;
            public FontWeight Weight;
            public bool Italic;

            public override bool Equals(object obj)
            {
                return obj is FontDesc desc && Equals(desc);
            }

            public bool Equals(TypefaceDesc other)
            {
                return FamilyName == other.FamilyName &&
                       Weight == other.Weight &&
                       Italic == other.Italic;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(FamilyName, Weight, Italic);
            }
        }

        private Dictionary<FontDesc, SkiaFont> fonts = new Dictionary<FontDesc, SkiaFont>();
        private Dictionary<string, List<SKTypeface>> typefaces = new Dictionary<string, List<SKTypeface>>();

        public Font FindFont(string familyName, float fontSize, FontWeight fontWeight, bool italic)
        {
            var fd = new FontDesc
            {
                Typeface = new TypefaceDesc
                {
                    FamilyName = familyName,
                    Weight = fontWeight,
                    Italic = italic
                },
                Size = (int)(fontSize * 10)
            };

            if(!fonts.TryGetValue(fd, out var font))
            {
                if (!MatchTypeface(fd.Typeface, out SKTypeface typeface))
                {
                    typeface = SKTypeface.FromFamilyName(familyName, fontWeight.ToSkia(), SKFontStyleWidth.Normal, italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
                }

                var skFont = typeface.ToFont(fontSize);
                font = new SkiaFont(skFont);
                if (fontWeight == FontWeight.Bold && typeface.FontWeight < (int)SKFontStyleWeight.SemiBold)
                {
                    font.SKPaint.FakeBoldText = true;
                }
                
                fonts.Add(fd, font);
            }

            return font;
        }

        private bool MatchTypeface(TypefaceDesc desc, out SKTypeface typeface)
        {
            typeface = null;
            if (!typefaces.TryGetValue(desc.FamilyName, out var list)) return false;

            int weightDiff = int.MaxValue;

            foreach(var el in  list.Where(o => ((o.FontSlant == SKFontStyleSlant.Italic) ^ desc.Italic) == false))
            {
                var diff = Math.Abs(el.FontWeight - (int)desc.Weight);
                if(diff < weightDiff)
                {
                    typeface = el;
                    weightDiff = diff;
                }
            }

            if (typeface != null) return true;

            weightDiff = int.MaxValue;
            foreach (var el in list.Where(o => o.FontSlant == SKFontStyleSlant.Upright))
            {
                var diff = Math.Abs(el.FontWeight - (int)desc.Weight);
                if (diff < weightDiff)
                {
                    typeface = el;
                    weightDiff = diff;
                }
            }
            if (typeface != null) return true;

            return false;
        }

        public void LoadTTF(Stream stream)
        {
            var tf = SKFontManager.Default.CreateTypeface(stream);

            if(!typefaces.TryGetValue(tf.FamilyName, out var list))
            {
                list = new List<SKTypeface>();
                typefaces.Add(tf.FamilyName, list);
            }
            list.Add(tf);
        }
    }
}
