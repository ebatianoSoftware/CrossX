// MIT License - Copyright © 2019 ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Graphics;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Texture2D = SharpDX.Direct3D11.Texture2D;

namespace CrossX.DxCommon.Graphics
{
    internal class DxRenderTarget : RenderTarget, IDxTexture
    {
        public RenderTargetView RenderTargetView { get; }
        public DepthStencilView DepthStencilView { get; }
        public ShaderResourceView ShaderResourceView { get; }

        public bool IsDisposed { get; private set; }

        public Texture2D Texture { get; }

        public DxRenderTarget(Texture2D texture, int width, int height)
        {
            var device = texture.Device;
            var desc2 = new RenderTargetViewDescription
            {
                Format = texture.Description.Format,
                Dimension = RenderTargetViewDimension.Texture2D
            };
            desc2.Texture2D.MipSlice = 0;
            RenderTargetView = new RenderTargetView(texture.Device, texture, desc2);

            Width = width;
            Height = height;

            using (var depthBuffer = new Texture2D(texture.Device, new Texture2DDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
            }))
            {
                DepthStencilView = new DepthStencilView(texture.Device, depthBuffer);
            }
        }

        public DxRenderTarget(DxGraphicsDevice graphicsDevice, RenderTargetCreationOptions creationOptions)
        {
            if (creationOptions.Content.HasFlag(RenderTargetContent.Color))
            {
                var desc = new Texture2DDescription
                {
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = creationOptions.Content.HasFlag(RenderTargetContent.Hdr) ? Format.R16G16B16A16_UNorm : Format.B8G8R8A8_UNorm,
                    Height = creationOptions.Height,
                    Width = creationOptions.Width,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default
                };

                var device = graphicsDevice.D3dDevice;
                Texture = new Texture2D(device, desc);

                ShaderResourceView = new ShaderResourceView(Texture.Device, Texture);

                var desc2 = new RenderTargetViewDescription
                {
                    Format = Texture.Description.Format,
                    Dimension = RenderTargetViewDimension.Texture2D
                };
                desc2.Texture2D.MipSlice = 0;
                RenderTargetView = new RenderTargetView(Texture.Device, Texture, desc2);
            }

            Width = creationOptions.Width;
            Height = creationOptions.Height;

            if (creationOptions.Content.HasFlag(RenderTargetContent.Depth))
            {
                using (var depthBuffer = new Texture2D(graphicsDevice.D3dDevice, new Texture2DDescription
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = creationOptions.Width,
                    Height = creationOptions.Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                }))
                {
                    DepthStencilView = new DepthStencilView(graphicsDevice.D3dDevice, depthBuffer);
                }
            }
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                ShaderResourceView?.Dispose();
                Texture?.Dispose();
                RenderTargetView?.Dispose();
                DepthStencilView?.Dispose();
                IsDisposed = true;
            }
        }

        public void Recreate()
        {
            
        }

        public void Reset()
        {
            
        }

        public override void Dispose()
        {
            Dispose(true);
        }
    }
}
