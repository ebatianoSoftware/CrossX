namespace CrossX.Audio
{
    public abstract class AudioEmitter
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Forward { get; set; }
        public float DopplerScale { get; set; }
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }
        public float Rolloff { get; set; }
    }
}
