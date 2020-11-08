using CrossX.Forms.Controls;
using CrossX.Forms.Xml;
using CrossX.Graphics2D;
using CrossX.IoC;
using System;
using System.Drawing;

namespace CrossX.Forms.View
{
    internal class View: IControlParent, IControlsLoader
    {
        private readonly IObjectFactory objectFactory;
        private readonly IServicesProvider servicesProvider;

        public RectangleF ClientArea { get; }

        public SpriteBatch SpriteBatch { get; }

        public PrimitiveBatch PrimitiveBatch { get; }

        public IControlsLoader ControlsLoader => this;
        public Control Root { get; set; }

        public bool IsFinished { get; private set; }

        public FormsViewModel ViewModel { get; }

        public View(IObjectFactory objectFactory, IServicesProvider servicesProvider, FormsViewModel viewModel)
        {
            SpriteBatch = objectFactory.Create<SpriteBatch>();
            PrimitiveBatch = objectFactory.Create<PrimitiveBatch>();
            this.objectFactory = objectFactory;
            this.servicesProvider = servicesProvider;
            ViewModel = viewModel;
        }

        public Control Load(XNode node)
        {
            var type = TypeFromNode(node);
            var control = (Control)objectFactory.Create(type, this);
            control.Id = node.Attribute("Id");

            return control;
        }

        public void Draw(TimeSpan frameTime)
        {
            Root.Draw(frameTime);
        }

        public void Update(TimeSpan frameTime)
        {
            Root.Update(frameTime);
        }

        public void Close()
        {
            IsFinished = true;
        }

        private Type TypeFromNode(XNode node)
        {
            var ns = node.Namespace.Replace("clr-namespace:", "").Split(',');
            var name = node.Tag;
            return Type.GetType(ns[0] + '.' + name + ',' + ns[1]);

        }
    }
}
