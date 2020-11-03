using System;

namespace CrossX.Graphics.Shaders
{
    public interface IShadersRepository: IDisposable
    {
        VertexShader GetVertexShader(string name);
        PixelShader GetPixelShader(string name);

        void RegisterVertexShader(string name, VertexShader vertexShader);
        void RegisterPixelShader(string name, PixelShader pixelShader);

        void DisposeVertexShader(string name);
        void DisposePixelShader(string name);
    }
}
