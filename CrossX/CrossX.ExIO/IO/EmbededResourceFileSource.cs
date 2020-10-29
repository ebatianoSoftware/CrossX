using System.IO;
using System.Reflection;

namespace CrossX.IO
{
    public class EmbededResourceFileSource : IFileSource
    {
        private readonly Assembly assembly;
        private readonly string workingPath;

        public EmbededResourceFileSource(Assembly assembly, string workingPath)
        {
            this.assembly = assembly;
            this.workingPath = workingPath;
        }

        public Stream Open(string path)
        {
            path = workingPath + '.' + path.Replace("\\", "/").Replace("/", ".");
            path = path.Replace("..", ".");

            var stream = assembly.GetManifestResourceStream(path);
            if (stream == null) throw new FileNotFoundException();
            return stream;
        }
    }
}
