namespace CrossX.Graphics.Shaders
{
    public abstract class VertexShader<TConstStruct>: Shader<TConstStruct> where TConstStruct : struct
    {
        public abstract VertexContent VertexContent { get; }
    }
}
