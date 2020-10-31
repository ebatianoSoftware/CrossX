// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace CrossX.Data
{
    /// <summary>
    /// Raw image class.
    /// </summary>
    public sealed class RawImage
    {
        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Height of the image in pixels.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Pixel data.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// Pixel format.
        /// </summary>
        /// <value>The pixel format.</value>
        public PixelDataFormat PixelFormat => (PixelDataFormat)((int)_pixelDataFormat & 0x7f);

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:EbatianoSoftware.CrossX.RawMedia.RawImage"/> contains premultiplied data.
        /// </summary>
        /// <value><c>true</c> if is premultiplied; otherwise, <c>false</c>.</value>
        public bool IsPremultiplied => PixelFormat.HasFlag(PixelDataFormat.Premultiplied);

        /// <summary>
        /// Number of bytes per one line.
        /// </summary>
        public readonly int Stride;

        private readonly PixelDataFormat _pixelDataFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EbatianoSoftware.CrossX.RawMedia.RawImage"/> class.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="data">Data of an image.</param>
        /// <param name="format">Pixel format with premultiplied flag.</param>
        public RawImage(int width, int height, byte[] data, PixelDataFormat format)
        {
            Width = width;
            Height = height;
            Data = data;
            _pixelDataFormat = format;

            Stride = width * ((int)_pixelDataFormat & 0x7);
        }

        public RawImage(int width, int height, byte[] data, int stride, PixelDataFormat format)
        {
            Width = width;
            Height = height;
            Data = data;
            _pixelDataFormat = format;

            Stride = stride;
        }

        /// <summary>
        /// Clone this instance.
        /// </summary>
        /// <returns>New <see cref="RawImage"/> instance with copied data.</returns>
        public RawImage Clone()
        {
            var data = new byte[Data.Length];
            Array.Copy(Data, data, data.Length);

            return new RawImage(Width, Height, data, _pixelDataFormat);
        }

        /// <summary>
        /// Generates the premultiplied data.
        /// </summary>
        /// <returns>The premultiplied data buffer.</returns>
        public byte[] GetPremultipliedRgba()
        {
            if (IsPremultiplied && PixelFormat.HasFlag(PixelDataFormat.Format32bppRGBA)) return Data;

            var bytes = new byte[4*Width*Height];

            switch (PixelFormat & (~PixelDataFormat.Premultiplied))
            {
                case PixelDataFormat.Format32bppRGBA:
                    PremultiplyData(Data, bytes);
                    break;

                case PixelDataFormat.Format24bppRGB:
                    From24bppRGB(Data, bytes);
                    break;

                case PixelDataFormat.Format16bppRA:
                    From16bppRA(Data, bytes);
                    PremultiplyData(Data, bytes);
                    break;

                case PixelDataFormat.Format8bppR:
                    From8bppA(Data, bytes);
                    break;
            }


            return bytes;
            
        }

        private void From8bppA(byte[] data, byte[] bytes)
        {
            var len = data.Length;
            for (var idx = 0; idx < len; ++idx)
            {
                bytes[idx * 4 + 0] = data[idx];
                bytes[idx * 4 + 1] = data[idx];
                bytes[idx * 4 + 2] = data[idx];
                bytes[idx * 4 + 3] = 255;
            }
        }

        private void From16bppRA(byte[] data, byte[] bytes)
        {
            var len = data.Length / 2;
            for (var idx = 0; idx < len; ++idx)
            {
                bytes[idx * 4 + 0] = data[idx * 2 + 0];
                bytes[idx * 4 + 1] = data[idx * 2 + 0];
                bytes[idx * 4 + 2] = data[idx * 2 + 0];
                bytes[idx * 4 + 3] = data[idx * 2 + 1];
            }
        }

        private void From24bppRGB(byte[] data, byte[] bytes)
        {
            var len = data.Length / 3;
            for(var idx = 0; idx < len; ++idx)
            {
                bytes[idx * 4 + 3] = 255;
                bytes[idx * 4 + 2] = data[idx * 3 + 2];
                bytes[idx * 4 + 1] = data[idx * 3 + 1];
                bytes[idx * 4 + 0] = data[idx * 3 + 0];
            }
        }

        private void PremultiplyData(byte[] data, byte[] bytes)
        {
            for (var idx = 0; idx < data.Length; idx += 4)
            {
                var a = Data[idx + 3];

                bytes[idx + 0] = (byte)(Data[idx + 0] * a / 255);
                bytes[idx + 1] = (byte)(Data[idx + 1] * a / 255);
                bytes[idx + 2] = (byte)(Data[idx + 2] * a / 255);
                bytes[idx + 3] = a;
            }
        }
    }
}
