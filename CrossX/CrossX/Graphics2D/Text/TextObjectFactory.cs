// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrossX.Graphics2D.Text
{
    /// <summary>
    /// Creates text objects for given parameters.
    /// </summary>
    public class TextObjectFactory
    {
        private readonly int[,] partsBuffer = new int[4, 256];
        private readonly int[] linesWidth = new int[256];
        private readonly int[] lastWordLastIndex = new int[4];
        private readonly int[] charsPerLine = new int[256];
        private readonly HashSet<int> linesToJustify = new HashSet<int>();

        private readonly List<int>[] linesWords = new List<int>[256];

        public TextObjectFactory()
        {
            for (var idx = 0; idx < linesWords.Length; ++idx)
            {
                linesWords[idx] = new List<int>();
            }
        }

        /// <summary>
        /// Creates the new text object.
        /// </summary>
        /// <returns>New text object.</returns>
        /// <param name="font">Font to create text with.</param>
        /// <param name="text">Text to create object from.</param>
        /// <param name="fontHeight">Height of font in pixels.</param>
        /// <param name="maxWidth">Max width in pixels or 0 for no limit.</param>
        /// <param name="alignment">Alignment of text. <see cref=" TextAlignment"/>.</param>
        public TextObject CreateText(Font font, TextSource text, float fontHeight, int maxWidth = 0, TextAlignment alignment = TextAlignment.Left)
        {
            var vertices = new List<List<TextVertex>>();

            for (var idx = 0; idx < font.Textures.Length; ++idx)
            {
                vertices.Add(new List<TextVertex>());
            }
            var size = CreateText(font, vertices, text, fontHeight, maxWidth, alignment);
            return new TextObject(vertices, font, size);
        }

        /// <summary>
        /// Updates the text object with new parameters.
        /// </summary>
        /// <param name="textObject">Text object to update.</param>
        /// <param name="text">Text to update object with.</param>
        /// <param name="fontHeight">Font height in pixels.</param>
        /// <param name="maxWidth">Max width in pixels or 0 for no limit.</param>
        /// <param name="alignment">Alignment of text. <see cref="TextAlignment"/>.</param>
        /// <param name="font">The font to change to or null if old font should be used.</param>
        public void UpdateText(TextObject textObject, TextSource text, float fontHeight, int maxWidth = 0, TextAlignment alignment = TextAlignment.Left, Font font = null)
        {
            var vertices = textObject.Vertices;
            font = font ?? textObject.Font;

            while (font.Textures.Length > vertices.Count)
            {
                vertices.Add(new List<TextVertex>());
            }

            foreach (var vert in vertices) vert.Clear();

            var size = CreateText(font, vertices, text, fontHeight, maxWidth, alignment);
            textObject.Size = size;
            textObject.Font = font;
        }

        private Vector2 CreateText(Font font, List<List<TextVertex>> vertices, TextSource text, float fontHeight, int maxWidth, TextAlignment alignment)
        {
            var scaleX = font.FontInfo.OnePixelScale.X * fontHeight;
            var scaleY = font.FontInfo.OnePixelScale.Y * fontHeight;

            var position = new Vector2(0, 0);
            var textSize = new Vector2(0, 0);

            var lastChar = '\0';
            var lines = 0;
            var lastSpace = -1;
            var lastWordWidth = 0;
            var lineHeight = (int)Math.Ceiling(font.FontInfo.Height * scaleY);

            for (var idx = 0; idx < 256; ++idx) charsPerLine[idx] = 0;
            for (var idx = 0; idx < 256; ++idx) linesWords[idx].Clear();
            linesToJustify.Clear();

            for (var idx = 0; idx < text.Length; ++idx)
            {
                var ch = text[idx];

                if (maxWidth > 0 && position.X > maxWidth)
                {
                    for (var page = 0; page < font.FontInfo.Pages.Length; ++page)
                    {
                        while (vertices[page].Count > lastWordLastIndex[page])
                        {
                            vertices[page].RemoveAt(vertices[page].Count - 1);
                        }

                        partsBuffer[page, lines] = vertices[page].Count;
                    }

                    linesToJustify.Add(lines);
                    charsPerLine[lines] -= idx - lastSpace;

                    idx = lastSpace;
                    linesWidth[lines] = lastWordWidth;

                    position.X = 0;
                    position.Y += lineHeight;
                    lastChar = '\0';
                    lines++;
                    continue;
                }

                var gl = font.FontInfo[ch];

                if (ch == ' ')
                {
                    lastSpace = idx;
                    lastWordWidth = linesWidth[lines];
                    for (var page = 0; page < font.FontInfo.Pages.Length; ++page)
                    {
                        lastWordLastIndex[page] = vertices[page].Count;
                    }

                    linesWords[lines].Add(vertices[0].Count);
                }

                if (ch == '\r') continue;

                if (ch == '\n')
                {
                    position.X = 0;
                    position.Y += lineHeight;
                    lastChar = '\0';
                    lines++;
                    continue;
                }

                var kerning = font.FontInfo.GetKerning(lastChar, ch);
                lastChar = ch;

                var targetPos = position + new Vector2((gl.Offset.X + kerning) * scaleX, gl.Offset.Y * scaleY);
                var size = new Vector2(gl.Rect.Width * scaleX, gl.Rect.Height * scaleY);

                var listToAdd = vertices[gl.Page];

                listToAdd.Add(new TextVertex
                {
                    Position = new Vector2(targetPos.X, targetPos.Y),
                    TextureCoordinate = new Vector2(gl.Rect.Left, gl.Rect.Top)
                });

                listToAdd.Add(new TextVertex
                {
                    Position = new Vector2(targetPos.X, targetPos.Y + size.Y),
                    TextureCoordinate = new Vector2(gl.Rect.Left, gl.Rect.Bottom)
                });

                listToAdd.Add(new TextVertex
                {
                    Position = new Vector2(targetPos.X + size.X, targetPos.Y),
                    TextureCoordinate = new Vector2(gl.Rect.Right, gl.Rect.Top)
                });

                listToAdd.Add(new TextVertex
                {
                    Position = new Vector2(targetPos.X + size.X, targetPos.Y),
                    TextureCoordinate = new Vector2(gl.Rect.Right, gl.Rect.Top)
                });

                listToAdd.Add(new TextVertex
                {
                    Position = new Vector2(targetPos.X, targetPos.Y + size.Y),
                    TextureCoordinate = new Vector2(gl.Rect.Left, gl.Rect.Bottom)
                });

                listToAdd.Add(new TextVertex
                {
                    Position = new Vector2(targetPos.X + size.X, targetPos.Y + size.Y),
                    TextureCoordinate = new Vector2(gl.Rect.Right, gl.Rect.Bottom)
                });

                partsBuffer[gl.Page, lines] = listToAdd.Count;
                charsPerLine[lines]++;

                position.X += (gl.Advance + kerning) * scaleX;
                linesWidth[lines] = (int)Math.Ceiling(position.X);
            }

            lines++;
            for (var idx = 0; idx < lines; ++idx)
            {
                textSize.X = Math.Max(textSize.X, linesWidth[idx]);
            }
            textSize.Y = lines * lineHeight;

            var targetWidth = Math.Max(maxWidth, (int)textSize.X);

            switch (alignment)
            {
                case TextAlignment.Center:
                    PositionText(font.FontInfo.Pages.Length, lines, vertices, targetWidth, CenterOffset);
                    break;

                case TextAlignment.Right:
                    PositionText(font.FontInfo.Pages.Length, lines, vertices, targetWidth, RightOffset);
                    break;

                case TextAlignment.Justify:
                    if (font.FontInfo.Pages.Length > 1) break;
                    JustifyText(lines, vertices, targetWidth);
                    break;
            }

            return textSize;
        }

        private void JustifyText(int lines, List<List<TextVertex>> vertices, int width)
        {
            var vert = vertices[0];

            for (var line = 0; line < lines; ++line)
            {
                var lastIndex = partsBuffer[0, line];
                var lineWidth = linesWidth[line];
                if (!linesToJustify.Contains(line)) continue;

                var words = linesWords[line];
                var spaces = words.Count;

                double spaceShift = (double)(width - lineWidth) / (spaces - 1);

                for (var sp = 0; sp < linesWords[line].Count; ++sp)
                {
                    var minIndex = words[sp];
                    var maxIndex = sp < words.Count - 1 ? words[sp + 1] : lastIndex;
                    var offset = spaceShift * (sp + 1);

                    for (var idx = minIndex; idx < maxIndex; ++idx)
                    {
                        var vertex = vert[idx];
                        var position = vertex.Position;
                        position.X += (float)offset;
                        vertex.Position = position;
                        vert[idx] = vertex;
                    }
                }
            }
        }

        private delegate int CalculateOffsetDelegate(int lineWidth, int width);

        private void PositionText(int pages, int lines, List<List<TextVertex>> vertices, int width,
            CalculateOffsetDelegate calculate)
        {
            for (var page = 0; page < pages; ++page)
            {
                var vert = vertices[page];

                for (var line = 0; line < lines; ++line)
                {
                    var lastIndex = partsBuffer[page, line];
                    var lineWidth = linesWidth[line];
                    var offset = calculate(lineWidth, width);

                    var firstIndex = line == 0 ? 0 : partsBuffer[page, line - 1];
                    for (var idx = firstIndex; idx < lastIndex; ++idx)
                    {
                        var vertex = vert[idx];
                        vertex.Position += new Vector2(offset, 0);
                        vert[idx] = vertex;
                    }
                }
            }
        }

        private int CenterOffset(int lineWidth, int width)
        {
            return (width - lineWidth) / 2;
        }

        private int RightOffset(int lineWidth, int width)
        {
            return width - lineWidth;
        }
    }
}
