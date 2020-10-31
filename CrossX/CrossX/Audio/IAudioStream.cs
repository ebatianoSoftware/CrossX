namespace CrossX.Audio
{
    public interface IAudioStream
    {
        int Channels { get; }
        int SampleRate { get; }
        int BitRate { get; }
        bool GetNextChunk(byte[] buffer, out int bytes);
        void ResetPosition();
    }
}
