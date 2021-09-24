namespace CrossX.Abstractions.Mvvm
{
    public interface IValueConverter
    {
        object Convert(object value, object parameter);
        object ConvertBack(object value, object parameter);
    }
}
