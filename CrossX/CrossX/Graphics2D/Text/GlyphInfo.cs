// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Drawing;

namespace CrossX.Graphics2D.Text
{
    /// <summary>
    /// Glyph info.
    /// </summary>
    public sealed class GlyphInfo
    {
        /// <summary>
        /// Texture rectangle, where glyph is placed.
        /// </summary>
        public readonly RectangleF Rect;

        /// <summary>
        /// Offset of glyph from TopLeft corner of current cursor position.
        /// </summary>
        public readonly Vector2 Offset;

        /// <summary>
        /// How much cursor advances in horizontal direction after this glyph.
        /// </summary>
        public readonly float Advance;

        /// <summary>
        /// Tha page on which the glyph is placed.
        /// </summary>
        public readonly int Page;

        public GlyphInfo(RectangleF rect, Vector2 offset, float advance, int page)
        {
            Rect = rect;
            Offset = offset;
            Advance = advance;
            Page = page;
        }
    }
}
