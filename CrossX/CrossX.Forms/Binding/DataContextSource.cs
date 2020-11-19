using CrossX.Forms.Controls;

namespace CrossX.Forms.Binding
{
    internal class DataContextSource : IValueSource
    {
        public readonly IObjectWithDataContext @object;

        public DataContextSource(IObjectWithDataContext @object)
        {
            this.@object = @object;
        }

        public object Resolve()
        {
            return @object.DataContext;
        }
    }
}
