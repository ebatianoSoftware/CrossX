using CrossX.Forms.Controls;
using CrossX.Forms.Converters;
using CrossX.Forms.Styles;
using CrossX.Forms.Xml;
using CrossX.Graphics;
using CrossX.Graphics2D;
using CrossX.IoC;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace CrossX.Forms.View
{
    internal class View: IControlParent, IControlsLoader
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;
        private readonly IDefaultConverters defaultConverters;

        public SpriteBatch SpriteBatch { get; }

        public PrimitiveBatch PrimitiveBatch { get; }

        public IControlsLoader ControlsLoader => this;
        public Control Root { get; set; }

        public bool IsFinished { get; private set; }

        public FormsViewModel ViewModel { get; }

        private bool shouldCalculateLayout;

        public View(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IDefaultConverters defaultConverters, FormsViewModel viewModel)
        {
            SpriteBatch = objectFactory.Create<SpriteBatch>();
            PrimitiveBatch = objectFactory.Create<PrimitiveBatch>();
            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
            this.defaultConverters = defaultConverters;
            ViewModel = viewModel;
        }

        public Control Load(XNode node, IControlParent parent)
        {
            var type = TypeFromNode(node);
            var control = (Control)objectFactory.Create(type, parent);
            control.Id = node.Attribute("Id");

            foreach(var ns in node.Nodes)
            {
                if (control is ContainerControl cc)
                {
                    cc.AddChild(Load(ns, cc));
                }
                else throw new InvalidDataException($"{type.Name} can't have child controls.");
            }

            LoadProperties(control, node);
            return control;
        }

        private void LoadProperties(Control control, XNode node)
        {
            var type = control.GetType();
            foreach(var attr in node.Attributes)
            {
                var prop = type.GetProperty(attr, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                if(prop != null)
                {
                    var valueStr = node.Attribute(attr);
                    object value = null;

                    if(prop.PropertyType.IsEnum)
                    {
                        value = Enum.Parse(prop.PropertyType, valueStr, true);
                    }
                    else if(prop.PropertyType == typeof(string))
                    {
                        value = valueStr;
                    }
                    else
                    {
                        var converter = defaultConverters.FindConverter(typeof(string), prop.PropertyType);
                        if (converter == null) throw new InvalidProgramException();
                        value = converter.Convert(valueStr);
                    }

                    prop.SetValue(control, value);
                }
                else
                {
                    control.SetCustomProperty(attr, node.Attribute(attr));
                }
            }
        }

        public void Draw(TimeSpan frameTime)
        {
            Root.Draw(frameTime);
        }

        public void Update(TimeSpan frameTime)
        {
            if(shouldCalculateLayout)
            {
                var screenSize = graphicsDevice.CurrentTargetSize;
                var clientArea = new RectangleF(0, 0, screenSize.Width, screenSize.Height);
                var size = Root.CalculateSize(clientArea);
                var position = Root.CalculatePosition(clientArea, size);
                Root.PositionControl(position, size);
            }
            Root.BeforeUpdate();
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

        public void InvalidateLayout()
        {
            shouldCalculateLayout = true;
        }
    }
}
