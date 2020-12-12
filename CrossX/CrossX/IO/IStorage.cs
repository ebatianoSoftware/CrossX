using System.IO;
using System.Threading.Tasks;

namespace CrossX.IO
{
    public interface IStorage
    {
        Task<Stream> OpenWrite(StorageSource source, string name);
        Task<Stream> OpenRead(StorageSource source, string name);
    }
}
