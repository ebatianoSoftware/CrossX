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
        TextureFilter TextureFilter { get; set; }
        RenderTarget RenderTarget { get; }
        Rectangle? ScissorsRect { get; set; }
        void BeginRender();
        void Present();
        void Clear(Color4 color);
        void SetRenderTarget(RenderTarget renderTarget);
        void SetVertexBuffer(VertexBuffer vertexBuffer);
        void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount);
        void Flush(object sender);
    }
}
