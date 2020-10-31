// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Graphics2D.Text
{
    /// <summary>
    /// Font style.
    /// </summary>
    [Flags]
    public enum FontStyle
    {
        /// <summary>
        /// Regular font style.
        /// </summary>
        Regular = 0,
        /// <summary>
        /// Bold font style.
        /// </summary>
        Bold = 1,
        /// <summary>
        /// Italic font style.
        /// </summary>
        Italic = 2,
        /// <summary>
        /// Bold &amp; Italic font style.
        /// </summary>
        BoldItalic = Bold | Italic
    }
}
