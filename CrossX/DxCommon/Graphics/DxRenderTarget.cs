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
        }

        public DxRenderTarget(DxGraphicsDevice graphicsDevice, RenderTargetCreationOptions creationOptions)
        {
            var desc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
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
                Dimension = RenderTargetViewDimension.Texture2D            };
            desc2.Texture2D.MipSlice = 0;
            RenderTargetView = new RenderTargetView(Texture.Device, Texture, desc2);

            Width = creationOptions.Width;
            Height = creationOptions.Height;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                ShaderResourceView?.Dispose();
                Texture?.Dispose();
                RenderTargetView?.Dispose();
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
