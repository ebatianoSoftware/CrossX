using System.IO;
using StbSharp;

namespace CrossX.Data
{
    public class ImagesFormat: IRawLoader<RawImage>
    {
        public static readonly IRawLoader<RawImage> Instance = new ImagesFormat();

        public RawImage FromStream(Stream stream)
        {
            byte[] bytes = null;

            // Rewind stream if it is at end
            if (stream.CanSeek && stream.Length == stream.Position)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }
            
            var image = StbImage.LoadFromMemory(bytes);
            
            PixelDataFormat format = 0;
            switch(image.Comp)
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
