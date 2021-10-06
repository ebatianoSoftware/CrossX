using CrossX.Framework.Binding;

namespace CrossX.Framework.UI.Controls
{
    public class CheckBox : CheckButtonBase
    {
        private bool @checked;

        [BindingMode(BindingMode.TwoWay)]
        public bool Checked { get => @checked; set => SetPropertyAndRedraw(ref @checked, value); }

        protected override bool IsChecked => Checked;

        public CheckBox(IUIServices services) : base(services)
        {

        }

        protected override void OnClick()
        {
            Checked = !Checked;
        }
    }
}
