using System.Runtime.InteropServices;

namespace CrossX.Graphics.Effects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Light
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Color4 Color;
        public Color4 Specular;
    }
}
