using CrossX.Graphics;
using CrossX.IO;
using S2IoC;

namespace CrossX.Content.Loaders
{
    public class TextureLoader : IContentLoader<Texture2D>
    {
        private readonly IFilesRepository filesRepository;
        private readonly IObjectFactory objectFactory;

        public TextureLoader(IFilesRepository filesRepository, IObjectFactory objectFactory)
        {
            this.filesRepository = filesRepository;
            this.objectFactory = objectFactory;
        }

        public Texture2D LoadContent(string path)
        {
            using (var stream = filesRepository.Open(path))
            {
                return objectFactory.Create<Texture2D>(stream);
            }
        }
    }
}
