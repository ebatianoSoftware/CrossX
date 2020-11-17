using CrossX.Forms.Binding;
using CrossX.Forms.Controls;
using CrossX.Forms.Converters;
using CrossX.Forms.Styles;
using CrossX.Forms.Xml;
using CrossX.Graphics;
using CrossX.Graphics2D;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CrossX.Forms.Views
{
    internal class View: IControlParent, IControlsLoader, IDisposable
    {
        private readonly IGraphicsDevice graphicsDevice;
        private readonly IObjectFactory objectFactory;

        private readonly IConverters defaultConverters;

        public SpriteBatch SpriteBatch { get; }

        public PrimitiveBatch PrimitiveBatch { get; }

        public IControlsLoader ControlsLoader => this;
        public Control Root { get; set; }

        public bool IsFinished { get; private set; }

        public FormsViewModel ViewModel { get; }

        public object DataContext => ViewModel;

        public IObjectFactory ObjectFactory { get; }

        private bool shouldCalculateLayout;

        private Size clientSize = Size.Empty;

        private Dictionary<string, Control> controlsWithId = new Dictionary<string, Control>();

        public View(IGraphicsDevice graphicsDevice, IObjectFactory objectFactory, IConverters defaultConverters, FormsViewModel viewModel)
        {
            SpriteBatch = objectFactory.Create<SpriteBatch>();
            PrimitiveBatch = objectFactory.Create<PrimitiveBatch>();
            ObjectFactory = objectFactory;

            this.graphicsDevice = graphicsDevice;
            this.objectFactory = objectFactory;
            this.defaultConverters = defaultConverters;
            ViewModel = viewModel;
        }

        public Control Load(XNode node)
        {
            var control = Load(node, this);
            control.RecreateBindings();
            return control;
        }

        public Control Load(XNode node, IControlParent parent)
        {
            var type = TypeFromNode(node);
            var control = (Control)objectFactory.Create(type, parent);
            control.Id = node.Attribute("Id");

            if (!string.IsNullOrEmpty(control.Id))
            {
                controlsWithId.Add(control.Id, control);
            }

            foreach(var ns in node.Nodes)
            {
                if (control is ContainerControl cc)
                {
                    cc.AddChild(Load(ns, cc));
                }
                else throw new InvalidDataException($"{type.Name} can't have child controls.");
            }

            LoadProperties(control, node);

            if (!control.BindingService.Contains(nameof(Control.DataContext)) && parent is Control parentAsControl)
            {
                control.BindingService.AddBinding(
                    new BindingDesc(typeof(Control).GetProperty(nameof(Control.DataContext)), 
                    new ParentSource(parentAsControl), 
                    nameof(Control.DataContext), null));
            }

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
                    
                    if(ParseBinding(prop, control, valueStr, out var binding))
                    {
                        control.BindingService.AddBinding(binding);
                        continue;
                    }

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
                        if (converter == null) throw new InvalidProgramException($"Cannot find converter string->{prop.PropertyType.Name}");
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

        private bool ParseBinding(PropertyInfo prop, Control control, string valueStr, out BindingDesc binding)
        {
            binding = null;
            if (!valueStr.StartsWith("{", StringComparison.OrdinalIgnoreCase)) return false;
            
            var parts = valueStr.Trim('{','}').Split('@');
            var source = ParseSource(parts.Length > 1 ? parts[1] : "", control);

            var nameParts = parts[0].Split(' ');
            
            string name = nameParts[0];
            IValueConverter converter = null;

            if (nameParts.Length > 1)
            {
                name = nameParts[1];
                converter = defaultConverters.FindConverter(nameParts[0]);
            }

            binding = new BindingDesc(prop, source, name, converter);
            return true;
        }

        private IValueSource ParseSource(string str, Control control)
        {
            if(str.StartsWith("ref:", StringComparison.Ordinal))
            {
                var id = str.Split(':')[1];
                return new ControlFromIdSource(this, id);
            }

            if(str.StartsWith("parent:", StringComparison.Ordinal))
            {
                var type = str.Split(':')[1];
                if (!type.Contains(',')) type = type + ",CrossX.Forms";

                return new ParentSource(control, Type.GetType(type));
            }

            if (str.StartsWith("parent", StringComparison.Ordinal))
            {
                return new ParentSource(control);
            }

            return new DataContextSource(control);
        }

        public void Draw(TimeSpan frameTime)
        {
            Root.Draw(frameTime);
        }

        public void Update(TimeSpan frameTime)
        {
            if(clientSize != graphicsDevice.CurrentTargetSize)
            {
                shouldCalculateLayout = true;
            }
            if(shouldCalculateLayout)
            {
                var screenSize = graphicsDevice.CurrentTargetSize;
                var clientArea = new RectangleF(0, 0, screenSize.Width, screenSize.Height);
                Root.CalculateSizeWithMargins(clientArea, out var size, out var _);
                var position = Root.CalculatePosition(clientArea, size);
                Root.PositionControl(position, size);
                clientSize = screenSize;
                shouldCalculateLayout = false;
            }
            Root.BeforeUpdate();
            Root.Update(frameTime);
        }

        public object FindControl(string id)
        {
            controlsWithId.TryGetValue(id, out var control);
            return control;
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

        public void Dispose()
        {
            Root.Dispose();
        }
    }
}
