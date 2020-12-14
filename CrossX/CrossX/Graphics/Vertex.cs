// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Runtime.InteropServices;

namespace CrossX.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPC
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Color;
        public Vector4 Position;
        public Color4 Color;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPCT
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Color | VertexContent.TextureCoordinates;
        public Vector4 Position;
        public Color4 Color;
        public Vector2 TextureCoordinate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPT
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.TextureCoordinates;
        public Vector4 Position;
        public Vector2 TextureCoordinate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPNC
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Normal | VertexContent.Color;
        public Vector4 Position;
        public Vector4 Normal;
        public Color4 Color;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPNCT
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Normal | VertexContent.Color | VertexContent.TextureCoordinates;
        public Vector4 Position;
        public Vector4 Normal;
        public Color4 Color;
        public Vector2 TextureCoordinate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPNT
    {
        public const VertexContent Content = VertexContent.Position | VertexContent.Normal | VertexContent.TextureCoordinates;
        public Vector4 Position;
        public Vector4 Normal;
        public Vector2 TextureCoordinate;
    }
}
