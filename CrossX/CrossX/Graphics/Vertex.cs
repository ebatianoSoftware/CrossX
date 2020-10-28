// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Drawing;
using System.Runtime.InteropServices;

namespace CrossX.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPC
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Color;
        public Vector3 Position;
        public Color Color;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPCT
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Color | VertexContent.TextureCoordinates;
        public Vector3 Position;
        public Color Color;
        public Vector2 TextureCoordinate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPT
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.TextureCoordinates;
        public Vector3 Position;
        public Vector2 TextureCoordinate;
    }
}
