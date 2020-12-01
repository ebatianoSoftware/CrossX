using CrossX.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CrossX.WindowsDx.IO
{
    public class Storage : IStorage
    {
        public Task<Stream> OpenRead(string name)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documents, name);

            return Task.FromResult((Stream)File.Open(path, FileMode.Open));
        }

        public Task<Stream> OpenWrite(string name)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var path = Path.Combine(documents, name);
            return Task.FromResult((Stream)File.Open(path, FileMode.Create));
        }
    }
}
