// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using SharpFNT;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CrossX.Graphics2D.Text
{
    /// <summary>
    /// All informations used to draw text with the font.
    /// </summary>
    public sealed class FontInfo
    {
        private readonly GlyphInfo[] ansiiCharacters = new GlyphInfo[128];
        private readonly Dictionary<char, GlyphInfo> extendedCharacters = new Dictionary<char, GlyphInfo>();

        private readonly Dictionary<long, float> kerning = new Dictionary<long, float>();

        /// <summary>
        /// Font face.
        /// </summary>
        public string Face { get; }

        /// <summary>
        /// Base line position from top of character in normalized (texture) units.
        /// </summary>
        public float Base { get; }

        /// <summary>
        /// Height of font in normalized (texture) units.
        /// </summary>
        public float Height { get; }
        /// <summary>
        /// Gets the font style.
        /// </summary>
        /// <value>The font style. <see cref="FontDesc.FontStyle"/>.</value>
        public FontStyle FontStyle { get; }

        /// <summary>
        /// Gets the height of the original pixel height of the font.
        /// </summary>
        /// <value>The height of font in pixels.</value>
        public int OriginalPixelHeight { get; }

        /// <summary>
        /// Scale to resize font to 1px height;
        /// </summary>
        /// <value>Decimal number to scale font to get exact 1px height.</value>
        public Vector2 OnePixelScale { get; }

        internal string[] Pages { get; }

        /// <summary>
        /// Gets the <see cref="T:EbatianoSoftware.CrossX.DrawText.FontDesc.FontInfo"/> for the specified character.
        /// </summary>
        /// <param name="character">Character information.</param>
        public GlyphInfo this[char character]
        {
            get
            {
                if (character < 128)
                {
                    return ansiiCharacters[character] ?? ansiiCharacters[32];
                }

                if (extendedCharacters.TryGetValue(character, out var glyph))
                {
                    return glyph;
                }

                return ansiiCharacters[32];
            }
        }

        internal FontInfo(BitmapFont bmFont)
        {
            Face = bmFont.Info.Face;
            FontStyle |= bmFont.Info.Bold ? FontStyle.Bold : 0;
            FontStyle |= bmFont.Info.Italic ? FontStyle.Italic : 0;

            Base = bmFont.Common.Base / (float)bmFont.Common.ScaleHeight;
            Pages = bmFont.Pages.Values.OrderBy(s => s).ToArray();

            OriginalPixelHeight = bmFont.Common.LineHeight;

            var aspect = bmFont.Common.ScaleWidth / (float)bmFont.Common.ScaleHeight;
            Height = bmFont.Common.LineHeight / (float)bmFont.Common.ScaleHeight;

            OnePixelScale = new Vector2((float)(aspect / Height), (float)(1.0 / Height));

            FillCharacters(bmFont);
            FillKerning(bmFont);
        }

        internal FontInfo(string face, int width, int height, int @base, int space, string characters, int textureWidth, int textureHeight)
        {
            Face = face;
            Pages = new[] { "" };

            float texHeight = textureHeight;
            float texWidth = textureWidth;

            Base = @base / texHeight;
            Height = height / texHeight;

            OriginalPixelHeight = height;
            OnePixelScale = new Vector2(texWidth / width, texHeight / height);

            FontStyle = FontStyle.Regular;

            int charsPerLine = textureWidth / width;

            int character = 0;
            float posX = 0.0f;
            float posY = 0.0f;


            ansiiCharacters[32] = new GlyphInfo(new RectangleF(0, 0, 0, 0), Vector2.Zero, (width + 1) / texWidth, 0);

            for (var idx = 0; idx < characters.Length; ++idx)
            {
                int ch = characters[idx];

                ansiiCharacters[ch] = new GlyphInfo(new RectangleF(posX / texWidth, posY / texHeight, width / texWidth, height / texHeight),
                    Vector2.Zero, (width + 1) / texWidth, 0);

                posX += width + space;
                character++;

                if (character == charsPerLine)
                {
                    character = 0;
                    posY += height + space;
                    posX = 0;
                }
            }
        }

        /// <summary>
        /// Gets the kerning for specified characters pair.
        /// </summary>
        /// <returns>The kerning.</returns>
        /// <param name="first">First character.</param>
        /// <param name="second">Second character.</param>
        public float GetKerning(char first, char second)
        {
            var key = (long)first << 32 | (long)second & 0xffffffff;

            if (!kerning.TryGetValue(key, out var amount)) amount = 0;
            return amount;
        }

        private void FillCharacters(BitmapFont bmFont)
        {
            float width = bmFont.Common.ScaleWidth;
            float height = bmFont.Common.ScaleHeight;

            foreach (var ch in bmFont.Characters)
            {
                if (ch.Key < 128)
                {
                    ansiiCharacters[ch.Key] = GlyphFromCharacter(width, height, ch.Value);
                }
                else
                {
                    var id = CharFromUnicode(ch.Key);
                    extendedCharacters.Add(id, GlyphFromCharacter(width, height, ch.Value));
                }
            }
        }

        private byte[] _idBytes = new byte[2];
        private char[] _charId = new char[1];

        private char CharFromUnicode(int value)
        {
            _idBytes[0] = (byte)(value & 0xff);
            _idBytes[1] = (byte)(value >> 8 & 0xff);
            Encoding.Unicode.GetChars(_idBytes, 0, 2, _charId, 0);
            return _charId[0];
        }

        private void FillKerning(BitmapFont bmFont)
        {
            if (bmFont.KerningPairs == null) return;

            float width = bmFont.Common.ScaleWidth;

            foreach (var p in bmFont.KerningPairs)
            {
                var first = CharFromUnicode(p.Key.First);
                var second = CharFromUnicode(p.Key.Second);

                var key = (long)first << 32 | (long)second & 0xffffffff;

                kerning.Add(key, p.Value / width);
            }
        }

        private GlyphInfo GlyphFromCharacter(float width, float height, Character character)
        {
            var rect = new RectangleF(character.X / width, character.Y / height,
                        character.Width / width, character.Height / height);

            var offset = new Vector2(character.XOffset / width, character.YOffset / height);
            var advance = character.XAdvance / width;

            return new GlyphInfo(rect, offset, advance, character.Page);
        }
    }
}
