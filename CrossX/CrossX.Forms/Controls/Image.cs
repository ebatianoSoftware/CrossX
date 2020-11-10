using CrossX.Forms.Values;
using System;

namespace CrossX.Forms.Controls
{
    public class Image : Control
    {
        private ImageSource source;
        private Stretch stretch;

        public ImageSource Source { get => source; set => SetProperty(ref source, value); }
        public Stretch Stretch { get => stretch; set => SetProperty(ref stretch, value); }

        public Image(IControlParent parent) : base(parent)
        {
        }

        public override void Draw(TimeSpan frameTime)
        {
            base.Draw(frameTime);
            if (Source.Texture == null) return;

            
        }
    }
}
