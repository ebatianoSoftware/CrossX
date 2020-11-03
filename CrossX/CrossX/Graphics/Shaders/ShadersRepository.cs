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

        public void DisposePixelShader(string name)
        {
            if(pixelShaders.TryGetValue(name, out var obj))
            {
                obj.Dispose();
                pixelShaders.Remove(name);
            }
        }

        public void DisposeVertexShader(string name)
        {
            if (vertexShaders.TryGetValue(name, out var obj))
            {
                obj.Dispose();
                vertexShaders.Remove(name);
            }
        }

        public PixelShader GetPixelShader(string name)
        {
            if (pixelShaders.TryGetValue(name, out var obj)) return obj as PixelShader;
            return null;
        }

        public VertexShader GetVertexShader(string name)
        {
            if (vertexShaders.TryGetValue(name, out var obj)) return obj as VertexShader;
            return null;
        }

        public void RegisterPixelShader(string name, PixelShader pixelShader)
        {
            pixelShaders.Add(name, pixelShader);
        }

        public void RegisterVertexShader(string name, VertexShader vertexShader)
        {
            vertexShaders.Add(name, vertexShader);
        }
    }
}
