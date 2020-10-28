// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Data
{
    /// <summary>
    /// Image pixel data format.
    /// </summary>
    [Flags]
    public enum PixelDataFormat
    {
        /// <summary>
        /// Red channel only.
        /// </summary>
        Format8bppR = 1,
        Format16bppRA = 2,
        /// <summary>
        /// 3 channels: R,G,B.
        /// </summary>
        Format24bppRGB = 3,

        /// <summary>
        /// 4 channels: R,G,B,A
        /// </summary>
        Format32bppRGBA = 4,

        /// <summary>
        /// Data is premultiplied with alpha.
        /// </summary>
        Premultiplied = 128
    }
}
