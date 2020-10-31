using System.IO;
using CrossX.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace CrossX.Media.Formats
{
    public sealed class ImagesFormat : IRawLoader<RawImage>
    {
        public static readonly IRawLoader<RawImage> Instance = new ImagesFormat();

        public RawImage FromStream(Stream stream)
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

                    data[position++] = pixel.R;
                    data[position++] = pixel.G;
                    data[position++] = pixel.B;
                    data[position++] = pixel.A;
                }
            }
            return new RawImage(image.Width, image.Height, data, PixelDataFormat.Format32bppRGBA);
        }
    }
}
