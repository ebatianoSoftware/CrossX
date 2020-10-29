// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Graphics
{
    /// <summary>
    /// Defines vertex content.
    /// </summary>
    [Flags]
    public enum VertexContent
    {
        /// <summary>
        /// Vertex contains position.
        /// </summary>
        Position = 1,
        /// <summary>
        /// Vertex contains color.
        /// </summary>
        Color = 2,
        /// <summary>
        /// Vertex contains texture coordinates.
        /// </summary>
        TextureCoordinates = 4,
        /// <summary>
        /// Vertex contains normal vector.
        /// </summary>
        Normal = 8,
    }
}