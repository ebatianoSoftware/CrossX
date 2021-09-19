namespace CrossX.Framework.UI.Controls
{
    public class ProgressBar : View
    {
        private float maxValue;
        private float progress;

        public float MaxValue { get => maxValue; set => SetProperty(ref maxValue, value); }
        public float Progress { get => progress; set => SetProperty(ref progress, value); }
    }
}
