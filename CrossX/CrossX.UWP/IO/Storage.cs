using CrossX.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CrossX.UWP.IO
{
    internal class Storage : IStorage
    {
        public async Task<Stream> OpenRead(StorageSource source, string name)
        {
            StorageFolder storageFolder = SpecialFolderFromSource(source);

            StorageFile sampleFile = await storageFolder.GetFileAsync(name);
            return await sampleFile.OpenStreamForReadAsync();
        }

        private StorageFolder SpecialFolderFromSource(StorageSource source)
        {
            switch(source)
            {
                case StorageSource.Documents:
                    return KnownFolders.DocumentsLibrary;

                case StorageSource.ApplicationData:
                    return ApplicationData.Current.LocalFolder;

                case StorageSource.ApplicationAssets:
                    return ApplicationData.Current.LocalFolder;
            }

            throw new ArgumentOutOfRangeException(nameof(source));
        }

        public async Task<Stream> OpenWrite(StorageSource source, string name)
        {
            StorageFolder storageFolder = SpecialFolderFromSource(source);

            StorageFile sampleFile = await storageFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            return await sampleFile.OpenStreamForWriteAsync();
        }
    }
}
