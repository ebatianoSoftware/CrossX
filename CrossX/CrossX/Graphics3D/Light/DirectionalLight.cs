using System.Numerics;
using System.Runtime.InteropServices;


namespace CrossX.Graphics3D.Light
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DirectionalLight
    {
        public Vector4 Direction;
        public Vector4 Color;
        public Vector4 Specular;
    }
}