using CrossX.IO;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CrossX.WindowsDx.IO
{
    public class Storage : IStorage
    {
        public Task<Stream> OpenRead(StorageSource source, string name)
        {
            string folder = SpecialFolderFromSource(source);
            var path = Path.Combine(folder, name);

            return Task.FromResult((Stream)File.Open(path, FileMode.Open));
        }

        private string SpecialFolderFromSource(StorageSource source)
        {
            switch (source)
            {
                case StorageSource.ApplicationData:
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                case StorageSource.ApplicationAssets:
                    return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Assets");

                case StorageSource.Documents:
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            return "";
        }

        public Task<Stream> OpenWrite(StorageSource source, string name)
        {
            var folder = SpecialFolderFromSource(source);

            var path = Path.Combine(folder, name);
            return Task.FromResult((Stream)File.Open(path, FileMode.Create));
        }
    }
}
