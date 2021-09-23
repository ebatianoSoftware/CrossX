using System.Threading.Tasks;

namespace CrossX.Framework.Graphics
{
    public interface IImageCache
    {
        Task<Image> GetImage(string uri);
    }
}
