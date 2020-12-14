using System;
using System.Numerics;

namespace CrossX.Audio
{
    public abstract class AudioListener
    {
        public virtual Vector3 Position { get; set; }
        public virtual Vector3 Velocity { get; set; }
        public virtual Vector3 Up { get; set; }
        public virtual Vector3 Forward { get; set; }

        public virtual event Action ValuesChanged;

        protected void OnValuesChanged() => ValuesChanged?.Invoke();
    }
}
