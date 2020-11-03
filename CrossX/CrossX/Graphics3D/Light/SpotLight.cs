using System.Runtime.InteropServices;

namespace CrossX.Graphics3D.Light
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpotLight
    {
        public PointLight PointLight;
        public Vector4 Direction;
        public float Cutoff;
        public float Unused0;
        public float Unused1;
        public float Unused2;
    }
}
