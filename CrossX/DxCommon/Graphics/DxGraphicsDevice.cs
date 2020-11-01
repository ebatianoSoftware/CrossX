// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using SharpDX.DXGI;
using SharpDX.Direct3D11;
using System;
using SharpDX.Mathematics.Interop;

using SdxDevice1 = SharpDX.Direct3D11.Device1;
using SdxResource = SharpDX.Direct3D11.Resource;
using CrossX.Graphics;
using System.Drawing;
using Texture2D = SharpDX.Direct3D11.Texture2D;
using CrossX.DxCommon.Helpers;
using CrossX.DxCommon.Graphics.Shaders;
using CrossX.Graphics.Shaders;

namespace CrossX.DxCommon.Graphics
{
    internal class DxGraphicsDevice : IGraphicsDevice, IDisposable
    {
        private ITargetWindow window;

        private SdxDevice1 d3dDevice;

        private int width;
        private int height;

        private bool fullscreen;

        private Texture2D backBuffer;
        private SwapChain1 swapChain;
        private DeviceContext1 D3dContext => d3dDevice.ImmediateContext1;
        private RenderTarget mainRenderTarget;

        public SdxDevice1 D3dDevice => d3dDevice;

        public RenderTarget RenderTarget { get; private set; }

        //public Rect ClipArea { get; set; }
        public Size Size => new Size(width, height);

        public event Action<Size> SizeChanged;
        public event EventHandler FlushRequest;

        public RenderStates RenderStates { get; private set; }

        public Size CurrentTargetSize => new Size(RenderTarget?.Width ?? width, RenderTarget?.Height ?? height);

        private BlendState1 blendState;
        private BlendMode blendMode;

        public bool DepthClip { get; set; }

        public BlendMode BlendMode
        {
            get => blendMode;
            set
            {
                if (blendMode == value) return;

                blendMode = value;
                switch (value)
                {
                    case BlendMode.Add:
                        blendState = RenderStates.AddBlendState;
                        break;

                    case BlendMode.AlphaBlend:
                        blendState = RenderStates.AlphaBlendState;
                        break;

                    case BlendMode.Multiply:
                        blendState = RenderStates.MultiplyBlendState;
                        break;

                    default:
                        blendState = RenderStates.NoBlendState;
                        break;
                }
            }
        }

        public Rectangle? ScissorsRect
        {
            get => scissorsRect;
            set
            {
                if (scissorsRect == value) return;
                Flush(this);
                scissorsRect = value;
            }
        }

        public IDxShader CurrentShader { get; internal set; }

        public void Initialize(ITargetWindow window, bool fullscreen)
        {
            this.window = window;
            this.fullscreen = fullscreen;

            SetupScreenBuffers();
            this.window.SizeChanged += (s, a) => SetupScreenBuffers();

            RenderStates = new RenderStates(this);
            RenderStates.Initialize();

            BlendMode = BlendMode.AlphaBlend;
        }

        private void SetupScreenBuffers()
        {
            width = window.Size.Width;
            height = window.Size.Height;

            backBuffer?.Dispose();
            mainRenderTarget?.Dispose();

            // If the swap chain already exists, resize it.
            if (swapChain != null)
            {
                swapChain.ResizeBuffers(2, width, height, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
                SizeChanged?.Invoke(Size);
            }
            // Otherwise, create a new one.
            else
            {
                swapChain = window.CreateSwapChain(width, height, fullscreen, out d3dDevice);
            }

            // Obtain the backbuffer for this window which will be the final 3D rendertarget.
            backBuffer = SdxResource.FromSwapChain<Texture2D>(swapChain, 0);
            mainRenderTarget = new DxRenderTarget(backBuffer, width, height);

            // Create a viewport descriptor of the full window size.
            var viewport = new RawViewportF
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                MinDepth = 0.0f,
                MaxDepth = 1.0f
            };

            // Set the current viewport using the descriptor.
            D3dContext.Rasterizer.SetViewport(viewport);
        }

        public void BeginRender()
        {
            SetRenderTarget(null);
        }

        public void Present()
        {
            Flush(this);
            D3dContext.Flush();
            swapChain.Present(1, PresentFlags.None);
        }

        public void Clear(Color4 color)
        {
            var rt = (DxRenderTarget)RenderTarget;

            D3dContext.ClearRenderTargetView(rt.RenderTargetView,
                new RawColor4(color.Rf, color.Gf, color.Bf, color.Af));
        }

        public void SetRenderTarget(RenderTarget renderTarget)
        {
            Flush(this);
            D3dContext.Flush();

            RenderTarget = renderTarget ?? mainRenderTarget;
            var rt = (DxRenderTarget)RenderTarget;
            D3dContext.OutputMerger.SetRenderTargets(rt.RenderTargetView);

            var viewport = new RawViewportF
            {
                X = 0,
                Y = 0,
                Width = RenderTarget.Width,
                Height = RenderTarget.Height,
                MinDepth = 0.0f,
                MaxDepth = 1.0f
            };

            // Set the current viewport using the descriptor.
            D3dContext.Rasterizer.SetViewport(viewport);
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
            var buffer = ((DxVertexBuffer)vertexBuffer).Buffer;
            var context = D3dDevice.ImmediateContext1;

            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(buffer, GeometryExtensions.StrideFromVertexContent(vertexBuffer.VertexContent), 0));
        }

        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            var context = D3dDevice.ImmediateContext1;
            if (indexBuffer is DxIndexBuffer2 dib2)
            {
                context.InputAssembler.SetIndexBuffer(dib2.Buffer, Format.R16_UInt, 0);
            }
            else if (indexBuffer is DxIndexBuffer4 dib4)
            {
                context.InputAssembler.SetIndexBuffer(dib4.Buffer, Format.R32_UInt, 0);
            }
        }

        private void PrepareRender(PrimitiveType primitiveType)
        {
            var context = D3dDevice.ImmediateContext1;
            context.InputAssembler.PrimitiveTopology = GeometryExtensions.PrimitiveTopologyFromPrimitiveType(primitiveType);

            if (ScissorsRect.HasValue)
            {
                var rect = ScissorsRect.Value;
                context.Rasterizer.State = DepthClip ? RenderStates.DepthClipRasterizerState : RenderStates.ClipRasterizerState;
                context.Rasterizer.SetScissorRectangle(rect.X, rect.Y, rect.Width, rect.Height);
            }
            else
            {
                context.Rasterizer.State = DepthClip ? RenderStates.DepthRasterizerState : RenderStates.RasterizerState;
            }

            context.OutputMerger.BlendState = blendState;
        }

        public void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount)
        {
            PrepareRender(primitiveType);
            var context = D3dDevice.ImmediateContext1;
            context.Draw(vertexCount, vertexStart);
        }

        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int indexStart, int indexCount)
        {
            PrepareRender(primitiveType);
            var context = D3dDevice.ImmediateContext1;
            context.DrawIndexed(indexCount, indexStart, 0);
        }

        public void SetShader<TConstStruct>(Shader<TConstStruct> shader) where TConstStruct : struct
        {
            ((IDxShader)shader).AttachShader();
        }

        public void SetPixelShaderSampler(int slot, TextureSamplerDesc samplerDesc)
        {
            var context = D3dDevice.ImmediateContext1;
            var sampler = RenderStates.GetSampler(samplerDesc);
            context.PixelShader.SetSampler(slot, sampler);
        }

        public void SetPixelShaderTexture(int slot, CrossX.Graphics.Texture2D texture)
        {
            var context = D3dDevice.ImmediateContext1;
            var dxTexture = texture as IDxTexture;
            context.PixelShader.SetShaderResource(slot, dxTexture?.ShaderResourceView);
        }

        public void Flush(object sender)
        {
            FlushRequest?.Invoke(sender, EventArgs.Empty);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private Rectangle? scissorsRect;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mainRenderTarget?.Dispose();
                    backBuffer?.Dispose();
                    swapChain?.Dispose();
                    d3dDevice?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DxGraphicsDevice() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
