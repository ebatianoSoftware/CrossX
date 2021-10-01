using CrossX.Framework.Binding;
using CrossX.Framework.Drawables;
using CrossX.Framework.Styles;

namespace CrossX.Framework.UI.Controls
{
    public class RadioButton : CheckButtonBase
    {
        // TODO: Make any value possible, not just ints
        private int currentValue;
        private int value;

        [BindingMode(BindingMode.TwoWay)]
        public int CurrentValue { get => currentValue; set => SetPropertyAndRedraw(ref currentValue, value); }
        public int Value { get => value; set => SetPropertyAndRedraw(ref this.value, value); }

        public RadioButton(IUIServices services) : base(services)
        {
            ApplyDefaultStyle();
        }

        protected override bool IsChecked => Value == CurrentValue;

        protected override void OnClick()
        {
            CurrentValue = Value;
        }

        protected override void ApplyDefaultStyle()
        {
            base.ApplyDefaultStyle();

            BoxDrawable = Services.AppValues.GetResource(ResourceValueKey.SystemRadioDrawable) as Drawable;
            TickMarkDrawable = Services.AppValues.GetResource(ResourceValueKey.SystemRadioTickDrawable) as Drawable;
        }
    }
}
