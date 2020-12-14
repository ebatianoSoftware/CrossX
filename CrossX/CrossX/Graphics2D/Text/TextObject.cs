// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Graphics;
using System.Collections.Generic;
using System.Drawing;

namespace CrossX.Graphics2D.Text
{
    public class TextObject
    {
        private Font _font;

        internal List<List<TextVertex>> Vertices { get; }

        internal Texture2D[] Textures { get; set; }

        public Vector2 Size { get; internal set; }

        internal Font Font
        {
            get => _font;
            set
            {
                _font = value;
                Textures = _font.Textures;
            }
        }

        private TextObject() { }

        internal TextObject(List<List<TextVertex>> vertices, Font font, Vector2 size)
        {
            Vertices = vertices;
            Size = size;
            Font = font;
        }
    }
}
