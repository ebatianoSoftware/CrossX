using System;

namespace CrossX.Forms.Transitions
{
    public class Transition
    {
        public bool IsFinished => time >= 1;

        public string Name { get; }

        private readonly float duration;
        private readonly bool inverted;
        private readonly Transform[] transforms;

        private float time = 0;

        public Transition(string name, float duration, bool inverted, Transform[] transforms)
        {
            Name = name;
            this.transforms = transforms;
            this.duration = duration;
            this.inverted = inverted;
        }

        public void Update(Vector2 origin, TimeSpan dt, out Matrix transform, out Color4 tint)
        {
            time += (float)dt.TotalSeconds / duration;
            if(time >= 1)
            {
                time = 1;
            }

            tint = Color4.White;
            transform = Matrix.Identity;

            var factor = inverted ? (1 - time) : time;

            for (var idx = 0; idx < transforms.Length; ++idx)
            {
                transforms[idx].Calculate(origin,factor, out var mat, out var color);
                tint *= color;
                transform *= mat;
            }
        }

        public Transition Clone(string name) => new Transition(name, duration, inverted, transforms);
    }
}
