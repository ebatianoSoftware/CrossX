using System.IO;
using CrossX.Data;
using StbSharp;

namespace CrossX.Media.Formats
{
    public class ImagesFormat : IRawLoader<RawImage>
    {
        public static readonly IRawLoader<RawImage> Instance = new ImagesFormat();

        public RawImage FromStream(Stream stream)
        {
            var reader = new ImageReader();
            var image = reader.Read(stream);

            PixelDataFormat format = 0;
            switch (image.Comp)
            {
                case 1: format = PixelDataFormat.Format8bppR; break;
                case 2: format = PixelDataFormat.Format16bppRA; break;
                case 3: format = PixelDataFormat.Format24bppRGB; break;
                case 4: format = PixelDataFormat.Format32bppRGBA; break;
            }

            return new RawImage(image.Width, image.Height, image.Data, format);
        }
    }
}
