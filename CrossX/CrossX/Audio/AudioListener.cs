using System;

namespace CrossX.Audio
{
    public abstract class AudioListener
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Forward { get; set; }

        public event Action ValuesChanged;

        protected void OnValuesChanged() => ValuesChanged?.Invoke();
    }
}
