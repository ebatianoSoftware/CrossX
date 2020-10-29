using System.IO;

namespace CrossX.IO
{
    public interface IFileSource
    {
        Stream Open(string path);
    }
}
