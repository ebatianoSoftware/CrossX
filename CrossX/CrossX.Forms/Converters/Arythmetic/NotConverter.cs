namespace CrossX.Forms.Converters.Arythmetic
{
    public class NotConverter : IValueConverter
    {
        public object Convert(object value)
        {
            if(value is bool b)
            {
                return !b;
            }

            return value;
        }
    }
}
