namespace CrossX.Framework.Binding
{

    public interface IBindingService
    {
        void AddBinding(BaseBinding binding);
        void RemoveBindings(object target);
        void AddDataContextBinding(object target, object source, string dataContextPropertyName);
    }
}
