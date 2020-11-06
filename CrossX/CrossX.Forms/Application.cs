using CrossX.Forms.Styles;
using CrossX.Forms.Xml;
using CrossX.IO;
using System.IO;
using System.Xml;

namespace CrossX.Forms
{
    internal class Application : IApplication
    {
        private readonly IStylesService stylesService;
        private readonly IFilesRepository filesRepository;

        public Application(IStylesService stylesService, IFilesRepository filesRepository)
        {
            this.stylesService = stylesService;
            this.filesRepository = filesRepository;
        }
        public void LoadStyles(string name)
        {
            XNode node;
            using(var stream = filesRepository.Open(name))
            {
                var reader = XmlReader.Create(stream);
                node = XNode.ReadXml(reader);
            }

            if (node.Tag != "Styles") throw new InvalidDataException();

            foreach(var cn in node.Nodes)
            {
                if(cn.Tag != "Style") throw new InvalidDataException();

            }
        }
    }
}
