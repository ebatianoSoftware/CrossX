using System.IO;

namespace CrossX.IO
{
    public interface IFilesRepository
    {
        Stream Open(string path);
    }
}
