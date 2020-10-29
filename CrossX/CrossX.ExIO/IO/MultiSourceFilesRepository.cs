using System.Collections.Generic;
using System.IO;

namespace CrossX.IO
{
    public class MultiSourceFilesRepository : IFilesRepository
    {
        public IFileSource DefaultSource { get; set; }

        private readonly Dictionary<string, IFileSource> fileSources = new Dictionary<string, IFileSource>();

        public Stream Open(string path)
        {
            var parts = path.Split('(');

            if(parts.Length == 1)
            {
                return DefaultSource.Open(path);
            }

            if (!fileSources.TryGetValue(parts[0], out var source)) throw new FileNotFoundException();
            return source.Open(parts[1].Trim(')'));
        }

        public void RegisterSource(string name, IFileSource source)
        {
            fileSources.Add(name, source);
        }
    }
}
