using System.IO;
using System.Threading.Tasks;

namespace CrossX.IO
{
    public interface IStorage
    {
        Task<Stream> OpenWrite(string name);
        Task<Stream> OpenRead(string name);
    }
}
