using System.Runtime.InteropServices;

namespace CrossX.Graphics3D.Light
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PointLight
    {
        public Vector4 Position;
        public Vector4 Color;
        public Vector4 Attenuation;
    }
}
