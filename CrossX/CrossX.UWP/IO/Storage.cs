using CrossX.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CrossX.UWP.IO
{
    internal class Storage : IStorage
    {
        public async Task<Stream> OpenRead(string name)
        {
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            StorageFile sampleFile = await storageFolder.GetFileAsync(name);
            return await sampleFile.OpenStreamForReadAsync();
        }

        public async Task<Stream> OpenWrite(string name)
        {
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            StorageFile sampleFile = await storageFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            return await sampleFile.OpenStreamForWriteAsync();
        }
    }
}
