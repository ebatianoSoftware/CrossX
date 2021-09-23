using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace CrossX.Framework.Binding
{
    public class BaseBinding: IDisposable
    {
        public object Target { get; }
        public PropertyInfo TargetProperty { get; }
        public string SourcePropertyName { get; }
        public object Source { get; private set; }
        public PropertyInfo SourceProperty { get; private set; }
        
        public BaseBinding(object target, PropertyInfo targetProperty, object source, string sourcePropertyName)
        {
            Target = target;
            TargetProperty = targetProperty;
            SourcePropertyName = sourcePropertyName;

            ChangeDataContext(source);
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Debug.Assert(sender == Source);

            if(args.PropertyName == SourceProperty?.Name)
            {
                SourceToTarget();
            }
        }

        private void SourceToTarget()
        {
            try
            {
                TargetProperty.SetValue(Target, SourceProperty.GetValue(Source));
            }
            catch
            {
            }
        }

        protected void ChangeDataContext(object context)
        {
            if (context == Source) return;

            if(Source is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= OnSourcePropertyChanged;
            }

            Source = context;
            SourceProperty = null;

            if (Source != null)
            {
                SourceProperty = Source.GetType().GetProperty(SourcePropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                if(SourceProperty == null)
                {
                    Source = null;
                }
            }

            if (Source is INotifyPropertyChanged notifyPropertyChanged2)
            {
                notifyPropertyChanged2.PropertyChanged += OnSourcePropertyChanged;
            }
            if (Source != null)
            {
                SourceToTarget();
            }
        }

        public virtual void Dispose()
        {
            ChangeDataContext(null);
        }
    }
}
