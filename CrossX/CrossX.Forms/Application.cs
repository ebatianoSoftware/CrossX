using CrossX.Forms.Styles;
using CrossX.Forms.Xml;
using CrossX.IO;
using System;
using System.IO;
using System.Xml;

namespace CrossX.Forms
{
    internal class Application : IApplication
    {
        private readonly IStylesService stylesService;
        private readonly IFilesRepository filesRepository;

        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan DeltaTime { get; private set; } = TimeSpan.Zero;

        public event Action<TimeSpan> BeforeUpdate;
        public event Action<TimeSpan> AfterUpdate;

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

        public void RaiseBeforeUpdate(TimeSpan time)
        {
            TotalTime += time;
            DeltaTime = time;
            BeforeUpdate?.Invoke(time);
        }
        public void RaiseAfterUpdate(TimeSpan time) => AfterUpdate?.Invoke(time);
    }
}
