// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Data;
using CrossX.IO;
using CrossX.Media.Formats;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrossX.DxCommon.Graphics
{
    internal class DxTexture : CrossX.Graphics.Texture2D, IDxTexture
    {
        public readonly Texture2D Texture;
        public ShaderResourceView ShaderResourceView { get; }

        public bool IsDisposed { get; private set; }

        public DxTexture(DxGraphicsDevice graphicsDevice, RawImage img) : this(graphicsDevice, img, false) { }

        public DxTexture(DxGraphicsDevice graphicsDevice, RawImage img, bool generateMipMaps)
        {
            var desc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Height = img.Height,
                Width = img.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            unsafe
            {
                fixed (byte* p = img.GetPremultipliedRgba())
                {
                    var ptr = (IntPtr)p;

                    var data = new DataBox(ptr, img.Stride, 0);
                    Texture = new Texture2D(graphicsDevice.D3dDevice, desc, new[] { data });

                    Width = img.Width;
                    Height = img.Height;
                }
            }

            try
            {
                ShaderResourceView = new ShaderResourceView(Texture.Device, Texture);
            }
            catch
            {
                ShaderResourceView = null;
                this.Texture = null;
            }

            if (generateMipMaps)
            {
                Texture.Device.ImmediateContext.GenerateMips(ShaderResourceView);
            }
        }

        public DxTexture(DxGraphicsDevice graphicsDevice, RawImage[] img)
        {
            var desc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Height = img[0].Height,
                Width = img[0].Width,
                MipLevels = img.Length,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            var data = new DataBox[img.Length];

            unsafe
            {
                var premultipliedBuffers = new List<byte[]>();

                for (var idx = 0; idx < img.Length; ++idx)
                {
                    premultipliedBuffers.Add(img[idx].GetPremultipliedRgba());

                    fixed (byte* p = premultipliedBuffers.Last())
                    {
                        var ptr = (IntPtr)p;
                        data[idx] = new DataBox(ptr, img[idx].Stride, 0);
                    }
                }
            }

            Texture = new Texture2D(graphicsDevice.D3dDevice, desc, data);
            Width = img[0].Width;
            Height = img[0].Height;

            try
            {
                ShaderResourceView = new ShaderResourceView(Texture.Device, Texture);
            }
            catch
            {
                ShaderResourceView = null;
                this.Texture = null;
            }
        }

        public DxTexture(DxGraphicsDevice graphicsDevice, Stream stream, bool generateMipMaps, IImageLoader loader = null) 
            : this(graphicsDevice, loader?.FromStream(stream, true) ?? ImagesFormat.Instance.FromStream(stream, true), generateMipMaps)
        {
        }

        public DxTexture(DxGraphicsDevice graphicsDevice, Stream stream, IImageLoader loader = null) 
            : this(graphicsDevice, loader?.FromStream(stream, true) ?? ImagesFormat.Instance.FromStream(stream, true), false)
        {
        }

        public DxTexture(DxGraphicsDevice graphicsDevice, string path, IFilesRepository filesRepository, IImageLoader loader = null)
            : this(graphicsDevice, new Func<RawImage>(()=>
            {
                RawImage image;
                using (var stream = filesRepository.Open(path))
                {
                    image = loader?.FromStream(stream, true) ?? ImagesFormat.Instance.FromStream(stream, true);
                }
                return image;
            }).Invoke(), false)
        {
        }

        public DxTexture(Texture2D texture, int width, int height, bool generateMipMaps)
        {
            this.Texture = texture;

            Width = width;
            Height = height;

            try
            {
                ShaderResourceView = new ShaderResourceView(texture.Device, texture);
            }
            catch
            {
                ShaderResourceView = null;
                this.Texture = null;
            }

            if (generateMipMaps)
            {
                texture.Device.ImmediateContext.GenerateMips(ShaderResourceView);
            }
        }

        public void Recreate()
        {
            
        }

        public void Reset()
        {
            
        }

        #region IDisposable Support
        protected bool DisposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    Texture?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                DisposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DxTexture() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
