using System;
using System.Collections.Generic;

namespace CrossX.Graphics.Shaders
{
    internal class ShadersRepository : IShadersRepository
    {
        private readonly Dictionary<string, IDisposable> vertexShaders = new Dictionary<string, IDisposable>();
        private readonly Dictionary<string, IDisposable> pixelShaders = new Dictionary<string, IDisposable>();

        public void Dispose()
        {
            foreach(var ps in pixelShaders)
            {
                ps.Value.Dispose();
            }
            pixelShaders.Clear();

            foreach (var vs in vertexShaders)
            {
                vs.Value.Dispose();
            }
            vertexShaders.Clear();
        }

        public void DisposePixelShader<T>(string name) where T : struct
        {
            if(pixelShaders.TryGetValue(name, out var obj))
            {
                obj.Dispose();
                pixelShaders.Remove(name);
            }
        }

        public void DisposeVertexShader<T>(string name) where T : struct
        {
            if (vertexShaders.TryGetValue(name, out var obj))
            {
                obj.Dispose();
                vertexShaders.Remove(name);
            }
        }

        public PixelShader<T> GetPixelShader<T>(string name) where T : struct
        {
            if (pixelShaders.TryGetValue(name, out var obj)) return obj as PixelShader<T>;
            return null;
        }

        public VertexShader<T> GetVertexShader<T>(string name) where T : struct
        {
            if (vertexShaders.TryGetValue(name, out var obj)) return obj as VertexShader<T>;
            return null;
        }

        public void RegisterPixelShader<T>(string name, PixelShader<T> pixelShader) where T : struct
        {
            pixelShaders.Add(name, pixelShader);
        }

        public void RegisterVertexShader<T>(string name, VertexShader<T> vertexShader) where T : struct
        {
            vertexShaders.Add(name, vertexShader);
        }
    }
}
