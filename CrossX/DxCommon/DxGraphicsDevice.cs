// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using SharpDX.DXGI;
using SharpDX.Direct3D11;
using System;
using SharpDX.Mathematics.Interop;
using EbatianoSoftware.CrossX.Graphics;

using SdxDevice1 = SharpDX.Direct3D11.Device1;
using SdxResource = SharpDX.Direct3D11.Resource;
using CrossX.Graphics;
using System.Drawing;
using Texture2D = SharpDX.Direct3D11.Texture2D;

namespace CrossX.DxCommon.Graphics
{
    internal class DxGraphicsDevice: IGraphicsDevice, IDisposable
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

        private RenderStates renderStates;
        private SamplerState samplerState;

        public Size CurrentTargetSize => new Size(RenderTarget?.Width ?? width, RenderTarget?.Height ?? height);

        //public IRenderTarget RenderTarget { get; private set; }

        private BlendState1 blendState;
        private BlendMode blendMode;

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
                        blendState = renderStates.AddBlendState;
                        break;

                    case BlendMode.AlphaBlend:
                        blendState = renderStates.AlphaBlendState;
                        break;

                    case BlendMode.Multiply:
                        blendState = renderStates.MultiplyBlendState;
                        break;

                    default:
                        blendState = renderStates.NoBlendState;
                        break;
                }
            }
        }

        //public Rect? ScissorsRect { get; set; }

        //public IDxShader CurrentShader { get; internal set; }

        private TextureFilter textureFilter;
        public TextureFilter TextureFilter
        {
            get => textureFilter;
            set
            {
                if (value == textureFilter) return;
                textureFilter = value;
                switch (value)
                {
                    case TextureFilter.Nearest:
                        samplerState = renderStates.NearestSamplerState;
                        break;

                    case TextureFilter.Linear:
                        samplerState = renderStates.LinearSamplerState;
                        break;

                    case TextureFilter.Anisotropic:
                        samplerState = renderStates.AnisotropicSamplerState;
                        break;
                }
            }
        }

        public void Initialize(ITargetWindow window, bool fullscreen)
        {
            this.window = window;
            this.fullscreen = fullscreen;

            SetupScreenBuffers();
            this.window.SizeChanged += (s, a) => SetupScreenBuffers();

            renderStates = new RenderStates(this);
            renderStates.Initialize();

            TextureFilter = TextureFilter.Nearest;
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
            D3dContext.Flush();
            swapChain.Present(1, PresentFlags.None);
        }

        public void Clear(Color color)
        {
            var rt = (DxRenderTarget)RenderTarget;

            D3dContext.ClearRenderTargetView(rt.RenderTargetView,
                new RawColor4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f));
        }

        public void SetRenderTarget(RenderTarget renderTarget)
        {
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

        //public void SetVertexBuffer(IVertexBuffer vertexBuffer)
        //{
        //    var buffer = vertexBuffer.UnderlyingObject<SharpDX.Direct3D11.Buffer>();
        //    var context = D3dDevice.ImmediateContext1;
        //    var effect = CurrentShader.EffectForContent(vertexBuffer.VertexContent);

        //    context.InputAssembler.InputLayout = effect.InputLayout;
        //    context.VertexShader.Set(effect.VertexShader);
        //    context.PixelShader.Set(effect.PixelShader);

        //    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(buffer, GeometryExtensions.StrideFromVertexContent(vertexBuffer.VertexContent), 0));
        //}

        //public void DrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount)
        //{
        //    var context = D3dDevice.ImmediateContext1;
        //    context.InputAssembler.PrimitiveTopology = GeometryExtensions.PrimitiveTopologyFromPrimitiveType(primitiveType);

        //    CurrentShader.ApplyShaderParameters();
        //    context.PixelShader.SetSampler(0, samplerState);

        //    if (ScissorsRect.HasValue)
        //    {
        //        var rect = ScissorsRect.Value;
        //        context.Rasterizer.SetScissorRectangle(rect.X, rect.Y, rect.Right, rect.Bottom);
        //        context.Rasterizer.State = renderStates.ClipRasterizerState;
        //    }
        //    else
        //    {
        //        context.Rasterizer.State = renderStates.RasterizerState;

        //    }

        //    context.OutputMerger.BlendState = blendState;
        //    context.Draw(vertexCount, vertexStart);
        //}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        

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
