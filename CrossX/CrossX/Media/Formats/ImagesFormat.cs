using System.IO;
using CrossX.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace CrossX.Media.Formats
{
    public sealed class ImagesFormat : IImageLoader
    {
        public static readonly IImageLoader Instance = new ImagesFormat();

        public RawImage FromStream(Stream stream)
        {
            return FromStream(stream, false);
        }

        public RawImage FromStream(Stream stream, bool premultiply)
        {
            var image = Image.Load<Rgba32>(stream);
            var data = new byte[image.Width * image.Height * 4];
            var stride = 4 * image.Width;

            for (int y = 0; y < image.Height; y++)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);
                var position = y * stride;
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = pixelRowSpan[x];

                    var mul = premultiply ? pixel.A / 255.0 : 1.0;

                    data[position++] = (byte)(pixel.R * mul);
                    data[position++] = (byte)(pixel.G * mul);
                    data[position++] = (byte)(pixel.B * mul);
                    data[position++] = pixel.A;
                }
            }
            return new RawImage(image.Width, image.Height, data, PixelDataFormat.Format32bppRGBA | (premultiply ? PixelDataFormat.Premultiplied : 0));
        }
    }
}
