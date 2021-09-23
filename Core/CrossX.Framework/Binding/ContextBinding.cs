using System.ComponentModel;
using System.Reflection;

namespace CrossX.Framework.Binding
{
    public class ContextBinding: BaseBinding
    {
        public BindingMode BindingMode { get; }
        public PropertyInfo TargetBindingContextProperty { get; }

        public ContextBinding(object target, PropertyInfo targetProperty, string bindingContextPropertyName, string sourcePropertyName, BindingMode mode)
            : base(target, targetProperty, null, sourcePropertyName)
        {
            BindingMode = mode;

            TargetBindingContextProperty = target.GetType().GetProperty(bindingContextPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);

            if (Target is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += OnTargetPropertyChanged;
            }

            ChangeDataContext(TargetBindingContextProperty.GetValue(target));
        }

        private void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == TargetBindingContextProperty.Name)
            {
                var newContext = TargetBindingContextProperty.GetValue(Target);
                ChangeDataContext(newContext);
                return;
            }

            if (args.PropertyName == TargetProperty.Name && BindingMode == BindingMode.TwoWay)
            {
                TargetToSource();
            }
        }

        private void TargetToSource()
        {
            try
            {
                SourceProperty.SetValue(Source, TargetProperty.GetValue(Target));
            }
            catch
            {
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Target is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged -= OnTargetPropertyChanged;
            }
        }

    }
}
