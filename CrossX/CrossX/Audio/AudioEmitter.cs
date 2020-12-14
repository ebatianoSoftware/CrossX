using System.Numerics;

namespace CrossX.Audio
{
    public abstract class AudioEmitter
    {
        public virtual Vector3 Position { get; set; }
        public virtual Vector3 Velocity { get; set; }
        public virtual Vector3 Up { get; set; }
        public virtual Vector3 Forward { get; set; }
        public virtual float DopplerScale { get; set; }
        public virtual float MinDistance { get; set; }
        public virtual float MaxDistance { get; set; }
        public virtual float Rolloff { get; set; }
    }
}
