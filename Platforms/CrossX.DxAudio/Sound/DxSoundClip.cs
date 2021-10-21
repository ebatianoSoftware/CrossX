using CrossX.Audio.Sound;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System.IO;

namespace CrossX.DxAudio.Sound
{
    internal class DxSoundClip : SoundClip
    {
        private readonly XAudioEngine audioEngine;

        public AudioBuffer Buffer { get; private set; }
        public uint[] DecodedPacketsInfo { get; }
        public WaveFormat WaveFormat { get; }

        public DxSoundClip(Stream stream, XAudioEngine audioEngine)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var soundStream = new SoundStream(memoryStream);
            Buffer = new AudioBuffer{ Stream = soundStream, AudioBytes = (int)soundStream.Length, Flags = BufferFlags.EndOfStream };
            DecodedPacketsInfo = soundStream.DecodedPacketsInfo;
            WaveFormat = soundStream.Format;
            this.audioEngine = audioEngine;
        }

        public override SoundClipInstance CreateInstance()
        {
            return new DxSoundClipInstance(this, audioEngine);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
