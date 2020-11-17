using CrossX.Forms.Controls;

namespace CrossX.Forms.Binding
{
    internal class DataContextSource : IValueSource
    {
        public readonly Control control;

        public DataContextSource(Control control)
        {
            this.control = control;
        }

        public object Resolve()
        {
            return control.DataContext;
        }
    }
}
