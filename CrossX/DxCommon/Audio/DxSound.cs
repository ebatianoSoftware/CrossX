using CrossX.Async;
using CrossX.Audio;
using CrossX.Data;
using CrossX.Media.Formats;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.IO;

namespace CrossX.DxCommon.Audio
{
    internal class DxSound : Sound
    {
        public AudioBuffer Buffer { get; }
        public WaveFormat Format { get; }

        public event Action Disposed;

        public override TimeSpan Duration => TimeSpan.Zero;

        public bool IsDisposed => false;

        private readonly DxAudioManager audioManager;
        private readonly ISoundSettings soundSettings;
        private readonly IDispatcher dispatcher;
        private readonly SoundInstancesPool soundInstancesPool;
        
        public DxSound(DxAudioManager audioManager, RawSound rawSound, ISoundSettings soundSettings, IDispatcher dispatcher)
        {
            Buffer = new AudioBuffer
            {
                Stream = DataStream.Create(rawSound.Data, true, true),
                AudioBytes = rawSound.Data.Length,
                Flags = BufferFlags.None
            };

            Format = new WaveFormat(rawSound.SampleRate, rawSound.BitsPerSample, rawSound.Channels);
            this.audioManager = audioManager;
            this.soundSettings = soundSettings;
            this.dispatcher = dispatcher;
            soundInstancesPool = new SoundInstancesPool(this);
        }

        public DxSound(DxAudioManager audioManager, Stream stream, ISoundSettings soundSettings, IDispatcher dispatcher, IRawLoader<RawSound> loader = null) 
            : this(audioManager, loader?.FromStream(stream) ?? WaveFileFormat.Instance.FromStream(stream), soundSettings, dispatcher)
        {
        }

        public override void Dispose()
        {
            Disposed?.Invoke();
        }

        public override SoundInstance NewInstance()
        {
            return new DxSoundInstance(audioManager, soundSettings, this, dispatcher);
        }

        public override SoundInstance NewInstance(AudioEmitter audioEmitter, AudioListener audioListener)
        {
            return new DxSoundInstance(audioManager, soundSettings, this, dispatcher, (DxAudioEmitter)audioEmitter, (DxAudioListener)audioListener);
        }

        public override void Play()
        {
            soundInstancesPool.Get().Play();
        }
    }
}
