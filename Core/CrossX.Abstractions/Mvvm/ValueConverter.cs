namespace CrossX.Abstractions.Mvvm
{
    public abstract class ValueConverter<TFrom, TTo> : IValueConverter
    {
        protected abstract TTo Convert(TFrom value, object parameter);
        protected abstract TFrom ConvertBack(TTo value, object parameter);

        object IValueConverter.Convert(object value, object parameter)
        {
            if(value is TFrom from)
            {
                return Convert(from, parameter);
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, object parameter)
        {
            if(value is TTo to)
            {
                return ConvertBack(to, parameter);
            }
            return null;
        }
    }
}
