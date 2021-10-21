using CrossX.Abstractions.Mvvm;

namespace CrossX.Abstractions.Menu
{
    public class MenuItemBase : BindingContext
    {
        private bool visible = true;
        public bool Visible { get => visible; set => SetProperty(ref visible, value); }
    }
}
