using System.Reflection;

namespace CrossX.Graphics.Shaders
{
    public class CreateVertexShaderFromResource
    {
        public string Path { get; set; }
        public Assembly Assembly { get; set; }
        public VertexContent VertexContent { get; set; }
    }

    public class CreatePixelShaderFromResource
    {
        public string Path { get; set; }
        public Assembly Assembly { get; set; }
    }
}
