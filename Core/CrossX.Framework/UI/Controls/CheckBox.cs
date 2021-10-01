using CrossX.Framework.Binding;
using CrossX.Framework.Drawables;
using CrossX.Framework.Styles;

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
            ApplyDefaultStyle();
        }

        protected override void OnClick()
        {
            Checked = !Checked;
        }

        protected override void ApplyDefaultStyle()
        {
            base.ApplyDefaultStyle();

            BoxDrawable = Services.AppValues.GetResource(ResourceValueKey.SystemCheckBoxDrawable) as Drawable;
            TickMarkDrawable = Services.AppValues.GetResource(ResourceValueKey.SystemCheckBoxTickDrawable) as Drawable;
        }
    }
}
