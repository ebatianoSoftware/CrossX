using CrossX.Async;
using CrossX.Audio;
using SharpDX.XAudio2;
using System;

namespace CrossX.DxCommon.Audio
{
    internal class DxSoundInstance : SoundInstance
    {
        private readonly DxAudioManager audioManager;
        private readonly ISoundSettings soundSettings;
        private readonly DxSound sound;
        private readonly IDispatcher dispatcher;

        public override bool IsLooped { get; set; }
        public override double Pitch 
        { 
            get => pitch; 
            set
            {
                pitch = value;
                ApplyParameters();
            }
        }
        public override double Volume 
        { 
            get => volume;
            set 
            { 
                volume = value;
                ApplyParameters();
            }
        }
        public double AbsoluteVolume { get; set; }

        public bool IsDisposed => false;

        private SourceVoice voice;
        private bool disposed;
        private double volume = 1;
        private double pitch = 1;

        public override event Action Finished;

        private readonly float[] matrix = new float[4];

        private readonly DxAudioListener audioListener;
        private readonly DxAudioEmitter audioEmitter;

        public DxSoundInstance(DxAudioManager audioManager, ISoundSettings soundSettings, DxSound sound, IDispatcher dispatcher)
        {
            this.audioManager = audioManager;
            this.soundSettings = soundSettings;
            this.sound = sound;
            this.dispatcher = dispatcher;
            sound.Disposed += DxSound_Disposed;
            voice = new SourceVoice(audioManager.Xaudio2, sound.Format, true);
            voice.BufferEnd += Voice_BufferEnd;

            soundSettings.ParametersChanged += ApplyParameters;
        }

        public DxSoundInstance(DxAudioManager audioManager, ISoundSettings soundSettings, DxSound sound, IDispatcher dispatcher, DxAudioEmitter audioEmitter, DxAudioListener audioListener) 
            : this(audioManager, soundSettings, sound, dispatcher)
        {
            this.audioEmitter = audioEmitter;
            this.audioListener = audioListener;

            audioEmitter.ValuesChanged += ApplyParameters;
            audioListener.ValuesChanged += ApplyParameters;
        }

        private void DxSound_Disposed()
        {
            Dispose();
            sound.Disposed -= DxSound_Disposed;
        }

        public void ApplyParameters()
        {
            var volume = (float)(soundSettings.SoundVolume * Volume);
            float dopplerFactor = 1;
            if (audioListener != null && audioEmitter != null)
            {
                XAudio2Model.Calculate2(volume, audioListener, audioEmitter, matrix, out dopplerFactor);
            }
            else
            {
                matrix[0] = volume;
                matrix[1] = volume;
            }

            voice?.SetOutputMatrix(sound.Format.Channels, 2, matrix);
            voice?.SetFrequencyRatio((float)(Pitch * dopplerFactor));
        }

        public override void Dispose()
        {
            if (disposed) return;

            Stop();
            if (audioEmitter != null) audioEmitter.ValuesChanged -= ApplyParameters;
            if (audioListener != null) audioListener.ValuesChanged -= ApplyParameters;

            soundSettings.ParametersChanged -= ApplyParameters;

            voice.BufferEnd -= Voice_BufferEnd;

            var voiceToDestroy = voice;
            
            voice = null;
            disposed = true;

            dispatcher.BeginInvoke(() =>
            {
                voiceToDestroy.DestroyVoice();
                voiceToDestroy.Dispose();
            });
        }

        public override void Pause()
        {
            State = SoundState.Paused;
            voice?.Stop();
        }

        public override void Play()
        {
            voice?.Stop();
            voice?.FlushSourceBuffers();
            voice?.SubmitSourceBuffer(sound.Buffer, null);

            if (IsLooped)
            {
                voice?.SubmitSourceBuffer(sound.Buffer, null);
            }

            State = SoundState.Playing;
            ApplyParameters();
            voice?.Start();
        }

        public override void Resume()
        {
            if (State != SoundState.Paused) return;
            State = SoundState.Playing;
            ApplyParameters();
            voice?.Start();
        }

        public override void Stop()
        {
            State = SoundState.Stopped;
            voice?.Stop();
            voice?.FlushSourceBuffers();
        }

        private void Voice_BufferEnd(IntPtr obj)
        {
            if (IsLooped && voice?.State.BuffersQueued < 2)
            {
                voice?.SubmitSourceBuffer(sound.Buffer, null);
            }

            if (voice?.State.BuffersQueued == 0)
            {
                State = SoundState.Stopped;
                Finished?.Invoke();
            }
        }

    }
}