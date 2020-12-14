using CrossX.Forms.Binding;
using CrossX.Forms.Controls;
using CrossX.Forms.Converters;
using CrossX.Forms.Helpers;
using CrossX.Forms.Values;
using CrossX.Graphics2D;
using CrossX.Input;
using CrossX.Xml;
using XxIoC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Numerics;

namespace CrossX.Forms.Views
{
    internal class View : ObservableDataModel, IObjectWithDataContext, IControlParent, IControlServices, IControlsLoader, IDisposable
    {
        private readonly IUiHost uiHost;
        private readonly IObjectFactory objectFactory;

        private readonly IConverters converters;
        private readonly ITouchPanel touchPanel;
        private readonly IFormsInput formsInput;
        private readonly NavigationView navigation;

        public  IMouse Mouse { get; }

        public SpriteBatch SpriteBatch { get; }
        public PrimitiveBatch PrimitiveBatch { get; }
        public IControlsLoader ControlsLoader => this;
        public Control Root { get; set; }
        public bool IsFinished { get; private set; }
        public FormsViewModel ViewModel { get; }

        public ICommand UiButtonCommand { get => uiButtonCommand; set => SetProperty(ref uiButtonCommand, value); }

        public object DataContext => ViewModel;

        public IObjectFactory ObjectFactory { get; }

        private bool shouldCalculateLayout;

        private Size clientSize = Size.Empty;

        private Dictionary<string, Control> controlsWithId = new Dictionary<string, Control>();

        private static readonly UiButton[] AllUiButtons = Enum.GetValues(typeof(UiButton)).Cast<UiButton>().ToArray();

        public bool IsClosing { get => isClosing; private set => SetProperty(ref isClosing, value); }
        public IFocusable Focus { get => focus; set => SetProperty(ref focus, value); }

        public ITransitionsManager TransitionsManager { get; }

        public ITransform2D Transform2D { get; }

        public IFormsSounds Sounds { get; }
        public CursorType CursorType { private get; set; }

        private bool isClosing;
        private IFocusable focus;
        private ICommand uiButtonCommand;
        private readonly BindingService bindingService;

        public View(IUiHost uiHost, IObjectFactory objectFactory, IConverters converters, ITouchPanel touchPanel,
            IFormsInput formsInput, ITransitionsManager transitionsManager, NavigationView navigation,
            FormsViewModel viewModel, IFormsSounds sounds, IMouse mouse)
        {
            Transform2D = uiHost.Transform2D;
            SpriteBatch = objectFactory.Create<SpriteBatch>(Transform2D);
            PrimitiveBatch = objectFactory.Create<PrimitiveBatch>(Transform2D);
            ObjectFactory = objectFactory;
            TransitionsManager = transitionsManager;
            this.navigation = navigation;
            this.uiHost = uiHost;
            this.objectFactory = objectFactory;
            this.converters = converters;
            this.touchPanel = touchPanel;
            this.formsInput = formsInput;

            ViewModel = viewModel;

            touchPanel.PointerCaptured += TouchPanel_PointerCaptured;
            touchPanel.PointerDown += TouchPanel_PointerDown;
            touchPanel.PointerMove += TouchPanel_PointerMove;
            touchPanel.PointerRemoved += TouchPanel_PointerRemoved;
            touchPanel.PointerUp += TouchPanel_PointerUp;

            bindingService = new BindingService(this, converters);
            Sounds = sounds;
            Mouse = mouse;
            viewModel.CallNavigateTo();
        }

        private void TouchPanel_PointerUp(TouchPoint point) => Root.ProcessTouch(point.Id, TouchEvent.Up, CalculateTouchPosition(point.Position));

        private void TouchPanel_PointerRemoved(TouchPoint point) => Root.ProcessTouch(point.Id, TouchEvent.Remove, CalculateTouchPosition(point.Position));

        private void TouchPanel_PointerMove(TouchPoint point) => Root.ProcessTouch(point.Id, TouchEvent.Move, CalculateTouchPosition(point.Position));

        private void TouchPanel_PointerDown(TouchPoint point)
        {
            if (Root.TransitionInProgress) return;
            Root.ProcessTouch(point.Id, TouchEvent.Down, CalculateTouchPosition(point.Position));
        }

        private Vector2 CalculateTouchPosition(Vector2 point) => point;

        private void TouchPanel_PointerCaptured(long id, object capturedBy) => Root.OnPointerCaptured(id, capturedBy);

        public void LoadView(XNode node, string @event)
        {
            foreach (var cn in node.Nodes)
            {
                if (Root != null) throw new InvalidDataException("Only one root control for page.");
                Root = Load(cn);
            }

            LoadProperties(this, node, bindingService, (n, o) => { });
            bindingService.RecreateValues();
            InvalidateLayout();
            Root.TriggerEvent(@event);
        }

        private Control Load(XNode node)
        {
            var control = Load(node, this);
            control.RecreateBindings();
            return control;
        }

        public Control Load(XNode node, IControlParent parent)
        {
            var type = XmlHelpers.TypeFromNode(node);
            var control = (Control)objectFactory.Create(type, parent, this);
            var name = node.Attribute("meta:Name");

            if (!string.IsNullOrEmpty(name))
            {
                controlsWithId.Add(name, control);
            }

            foreach (var ns in node.Nodes)
            {
                if(ns.Tag.Contains('.'))
                {
                    var kind = ns.Tag.Split('.')[1];
                    control.ParseSpecial(kind, ns);
                }
                else  if (control is ContainerControl cc)
                {
                    cc.AddChild(Load(ns, cc));
                }
                else throw new InvalidDataException($"{type.Name} can't have child controls.");
            }

            LoadProperties(control, node);

            if (!control.BindingService.Contains(nameof(IObjectWithDataContext.DataContext)))
            {
                control.BindingService.AddBinding(
                    new BindingDesc(typeof(Control).GetProperty(nameof(IObjectWithDataContext.DataContext), BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty),
                    new ParentSource(control),
                    nameof(IObjectWithDataContext.DataContext), null));
            }

            return control;
        }

        private void LoadProperties(IObjectWithDataContext target, XNode node, BindingService bindingService, Action<string, object> setCustomProperty)
        {
            var type = target.GetType();
            foreach (var attr in node.Attributes)
            {
                var prop = type.GetProperty(attr, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                if (prop != null)
                {
                    var valueStr = node.Attribute(attr);

                    if (ParseBinding(prop, target, valueStr, out var binding))
                    {
                        bindingService.AddBinding(binding);
                        continue;
                    }

                    object value = null;

                    if (prop.PropertyType.IsEnum)
                    {
                        value = Enum.Parse(prop.PropertyType, valueStr, true);
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        value = valueStr;
                    }
                    else
                    {
                        var converter = converters.FindConverter(typeof(string), prop.PropertyType);
                        if (converter == null) throw new InvalidProgramException($"Cannot find converter string->{prop.PropertyType.Name}");
                        value = converter.Convert(valueStr);
                    }

                    prop.SetValue(target, value);
                }
                else
                {
                    setCustomProperty?.Invoke(attr, node.Attribute(attr));
                }
            }
        }

        private void LoadProperties(Control control, XNode node)
        {
            LoadProperties(control, node, control.BindingService, (n, o) => control.SetCustomProperty(n, o));
        }

        private bool ParseBinding(PropertyInfo prop, IObjectWithDataContext target, string valueStr, out BindingDesc binding)
        {
            binding = null;
            if (!valueStr.StartsWith("{", StringComparison.OrdinalIgnoreCase)) return false;

            var parts = valueStr.Trim('{', '}').Split('@');
            var source = ParseSource(parts.Length > 1 ? parts[1] : "", target);

            var nameParts = parts[0].Split(' ');

            string name = nameParts[0];
            IValueConverter converter = null;

            if (nameParts.Length > 1)
            {
                name = nameParts[1];
                converter = converters.FindConverter(nameParts[0]);
            }

            binding = new BindingDesc(prop, source, name, converter);
            return true;
        }

        private IValueSource ParseSource(string str, IObjectWithDataContext target)
        {
            if (str.StartsWith("ref:", StringComparison.Ordinal))
            {
                var id = str.Split(':')[1];
                return new ControlFromIdSource(this, id);
            }

            if (str.StartsWith("parent:", StringComparison.Ordinal))
            {
                var type = str.Split(':')[1];
                if (!type.Contains(',')) type = type + ",CrossX.Forms";

                if (target is Control control)
                {
                    return new ParentSource(control, Type.GetType(type));
                }
                throw new InvalidOperationException("Cannot set parent source to no control elements.");
            }

            if (str.StartsWith("parent", StringComparison.Ordinal))
            {
                if (target is Control control)
                {
                    return new ParentSource(control);
                }
                throw new InvalidOperationException("Cannot set parent source to no control elements.");
            }

            return new DataContextSource(target);
        }

        public void Draw(TimeSpan frameTime)
        {
            Root.Draw(frameTime, Color4.White);
        }

        public void Update(TimeSpan frameTime)
        {
            if (clientSize != uiHost.TargetRect.Size)
            {
                shouldCalculateLayout = true;
            }
            if (shouldCalculateLayout)
            {
                var screenSize = uiHost.TargetRect.Size;
                var clientArea = uiHost.TargetRect;
                Root.CalculateSizeWithMargins(clientArea, out var size, out var _);
                var position = Root.CalculatePosition(clientArea, size);
                Root.PositionControl(position, size);
                clientSize = screenSize;
                shouldCalculateLayout = false;
            }
            Root.BeforeUpdate();
            ProcessUiButtons();
            CursorType = CursorType.Arrow;
            Root.ProcessTouch(0, TouchEvent.Idle, Mouse.Position);
            Root.Update(frameTime);
            Mouse.Cursor = CursorType;

            if (IsClosing)
            {
                IsFinished = !Root.TransitionInProgress;
            }
        }

        private void ProcessUiButtons()
        {
            if (!navigation.IsTop(this)) return;
            if (Root.TransitionInProgress) return;

            var focusable = Focus;

            for (var idx = 0; idx < AllUiButtons.Length; ++idx)
            {
                var btn = AllUiButtons[idx];
                var state = formsInput.GetUiButtonState(btn);

                switch (state)
                {
                    case KeyBtnState.JustPressed:
                        if (focusable?.OnUiButtonPressed(btn) ?? false) return;
                        UiButtonCommand?.Execute(btn);
                        break;

                    case KeyBtnState.JustReleased:
                        if (focusable?.OnUiButtonReleased(btn) ?? false) return;
                        break;
                }
            }
        }

        public object FindControl(string id)
        {
            controlsWithId.TryGetValue(id, out var control);
            return control;
        }

        public void Close(string @event)
        {
            if (IsClosing) return;

            IsClosing = true;
            Root.TriggerEvent(@event);
            IsFinished = !Root.TransitionInProgress;
            ViewModel.CallNavigateFrom();
        }

        public void InvalidateLayout()
        {
            shouldCalculateLayout = true;
        }

        public void Dispose()
        {
            Root.Dispose();
            touchPanel.PointerCaptured -= TouchPanel_PointerCaptured;
            touchPanel.PointerDown -= TouchPanel_PointerDown;
            touchPanel.PointerMove -= TouchPanel_PointerMove;
            touchPanel.PointerRemoved -= TouchPanel_PointerRemoved;
            touchPanel.PointerUp -= TouchPanel_PointerUp;
        }
    }
}
