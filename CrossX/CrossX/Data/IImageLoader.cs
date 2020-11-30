using System.IO;

namespace CrossX.Data
{
    public interface IImageLoader: IRawLoader<RawImage>
    {
        RawImage FromStream(Stream stream, bool premultiply);
    }
}
