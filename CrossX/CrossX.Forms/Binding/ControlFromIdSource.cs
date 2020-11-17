using CrossX.Forms.Views;

namespace CrossX.Forms.Binding
{
    internal class ControlFromIdSource : IValueSource
    {
        private readonly View view;
        private readonly string id;

        public ControlFromIdSource(View view, string id)
        {
            this.view = view;
            this.id = id;
        }

        public object Resolve()
        {
            return view.FindControl(id);
        }
    }
}
