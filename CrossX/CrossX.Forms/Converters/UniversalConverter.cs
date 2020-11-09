using System;

namespace CrossX.Forms.Converters
{
    public class UniversalConverter<TFrom, TTo> : IValueConverter
    {
        private readonly Func<TFrom, TTo> convert;

        public UniversalConverter( Func<TFrom, TTo> convert )
        {
            this.convert = convert;
        }

        public object Convert(object value)
        {
            if(value is TFrom from)
            {
                return convert(from);
            }
            return default(TTo);
        }
    }
}
