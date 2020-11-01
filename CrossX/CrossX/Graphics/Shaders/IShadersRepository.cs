using System;

namespace CrossX.Graphics.Shaders
{
    public interface IShadersRepository: IDisposable
    {
        VertexShader<T> GetVertexShader<T>(string name) where T : struct;
        PixelShader<T> GetPixelShader<T>(string name) where T : struct;

        void RegisterVertexShader<T>(string name, VertexShader<T> vertexShader) where T : struct;
        void RegisterPixelShader<T>(string name, PixelShader<T> pixelShader) where T : struct;

        void DisposeVertexShader<T>(string name) where T : struct;
        void DisposePixelShader<T>(string name) where T : struct;
    }
}
