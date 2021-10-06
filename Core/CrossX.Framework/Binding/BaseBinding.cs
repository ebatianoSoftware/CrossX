using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace CrossX.Framework.Binding
{
    public class BaseBinding: IDisposable
    {
        private readonly IConversionService conversionService;

        public object Target { get; }
        public PropertyInfo TargetProperty { get; }
        public string SourcePropertyName { get; }
        public object Source { get; private set; }
        public PropertyInfo SourceProperty { get; private set; }
        
        public BaseBinding(object target, PropertyInfo targetProperty, object source, string sourcePropertyName, IConversionService conversionService)
        {
            Target = target;
            TargetProperty = targetProperty;
            SourcePropertyName = sourcePropertyName;
            this.conversionService = conversionService;
            ChangeDataContext(source);
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == SourceProperty?.Name)
            {
                SourceToTarget();
            }
        }

        private void SourceToTarget()
        {
            try
            {
                SetProperty(Target, TargetProperty, Source, SourceProperty);
            }
            catch
            {
            }
        }

        protected void SetProperty(object to, PropertyInfo toInfo, object from, PropertyInfo fromInfo)
        {
            object value = fromInfo?.GetValue(from) ?? from;
            value = conversionService.Convert(value, toInfo.PropertyType);

            if (value == null) return;

            toInfo.SetValue(to, value);
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
                if (string.IsNullOrWhiteSpace(SourcePropertyName))
                {
                    SourceProperty = null;
                }
                else
                {
                    SourceProperty = Source.GetType().GetProperty(SourcePropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    if (SourceProperty == null)
                    {
                        Source = null;
                    }
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
