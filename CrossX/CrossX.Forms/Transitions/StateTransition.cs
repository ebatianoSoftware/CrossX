using System;
using System.Numerics;

namespace CrossX.Forms.Transitions
{
    public class StateTransition
    {
        private readonly float duration;
        private float value = 0;
        private readonly bool inverted;

        private readonly Transform[] transforms;
        private bool firstUpdate = true;

        public bool State { get; set; }
        public string Name { get; }

        public StateTransition(string name, float duration, bool inverted, Transform[] transforms)
        {
            this.transforms = transforms;
            Name = name;
            this.duration = duration;
            this.inverted = inverted;
        }

        public void Update(Vector2 origin, TimeSpan dt, out Matrix4x4 transform, out Color4 tint)
        {
            if (firstUpdate)
            {
                value = inverted ^ State ? 1 : 0;
                firstUpdate = false;
            }

            var sign = inverted ^ State ? 1 : -1;
            
            value += (float)dt.TotalSeconds / duration * sign;
            value = Math.Min(1, Math.Max(0, value));

            tint = Color4.White;
            transform = Matrix4x4.Identity;

            for (var idx = 0; idx < transforms.Length; ++idx)
            {
                transforms[idx].Calculate(origin, value, out var mat, out var color);
                tint *= color;
                transform *= mat;
            }
        }

        public StateTransition Clone(string name) => new StateTransition(name, duration, inverted, transforms);
    }
}
