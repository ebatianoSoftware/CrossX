using CrossX.Audio;
using CrossX.IO;
using S2IoC;

namespace CrossX.Content.Loaders
{
    public class SoundLoader : IContentLoader<Sound>
    {
        private readonly IFilesRepository filesRepository;
        private readonly IObjectFactory objectFactory;

        public SoundLoader(IFilesRepository filesRepository, IObjectFactory objectFactory)
        {
            this.filesRepository = filesRepository;
            this.objectFactory = objectFactory;
        }

        public Sound LoadContent(string path)
        {
            using (var stream = filesRepository.Open(path))
            {
                return objectFactory.Create<Sound>(stream);
            }
        }
    }
}
