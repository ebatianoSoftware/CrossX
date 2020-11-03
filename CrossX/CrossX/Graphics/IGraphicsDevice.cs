using CrossX.Graphics.Shaders;
using System;
using System.Drawing;

namespace CrossX.Graphics
{
    public interface IGraphicsDevice: IDisposable
    {
        event EventHandler FlushRequest;

        Size Size { get; }
        Size CurrentTargetSize { get; }
        BlendMode BlendMode { get; set; }
        RenderTarget RenderTarget { get; }
        Rectangle? ScissorsRect { get; set; }
        bool DepthClip { get; set; }
        void Present();
        void Clear(Color4 color);
        void SetRenderTarget(RenderTarget renderTarget);
        void SetVertexBuffer(VertexBuffer vertexBuffer);
        void SetIndexBuffer(IndexBuffer indexBuffer);
        void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount);
        void DrawIndexedPrimitives(PrimitiveType primitiveType, int indexStart, int indexCount);
        void Flush(object sender);
        void SetShader(Shader shader);
        void SetPixelShaderSampler(int slot, TextureSamplerDesc samplerDesc);
        void SetPixelShaderTexture(int slot, Texture2D texture);
    }
}
