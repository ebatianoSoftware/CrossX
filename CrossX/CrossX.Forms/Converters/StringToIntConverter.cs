namespace CrossX.Forms.Converters
{
    internal class StringToIntConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if(value is string str)
            {
                int.TryParse(str, out var result);
                return result;
            }
            return 0;
        }
    }
}
