

using CrossX.Async;
using CrossX.Audio;
using CrossX.Utils;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;

namespace CrossX.DxCommon.Audio
{
    internal class DxAudioStreamPlayer : AudioStreamPlayer
    {
        private WaveFormat format;
        private readonly ISoundSettings soundSettings;
        private readonly IAudioStream stream;
        private readonly IDispatcher dispatcher;
        private SourceVoice voice;
        private bool disposed;
        private bool loop;

        private float volume;

        private Queue<byte[]> submitedBuffers = new Queue<byte[]>();
        private readonly ObjectPool<byte[]> arraysPool = new ObjectPool<byte[]>();
        
        public override float Volume 
        { 
            get
            {
                return volume;
            }

            set
            {
                volume = value;
                voice?.SetVolume(volume * soundSettings.MusicVolume);
            }
        }

        public override event Action Finished;

        public DxAudioStreamPlayer(DxAudioManager audioManager, ISoundSettings soundSettings, IAudioStream stream, IDispatcher dispatcher)
        {
            this.soundSettings = soundSettings;
            this.stream = stream;
            this.dispatcher = dispatcher;

            arraysPool.SetCustomFactory(() => new byte[524288]);

            format = new WaveFormat(stream.SampleRate, stream.BitRate, stream.Channels);
            voice = new SourceVoice(audioManager.Xaudio2, format);

            Volume = 1;
            voice.BufferEnd += Voice_BufferEnd;

            soundSettings.ParametersChanged += SoundSettings_ParametersChanged;

        }

        private void SoundSettings_ParametersChanged() => Volume = volume;

        private void Voice_BufferEnd(IntPtr id)
        {
            if (submitedBuffers.Count > 0)
            {
                arraysPool.Return(submitedBuffers.Dequeue());
            }

            SubmitBuffersIfNeeded();

            if (voice.State.BuffersQueued == 0)
            {
                Finished?.Invoke();
            }
        }

        private void SubmitBuffersIfNeeded()
        {
            while(voice.State.BuffersQueued < 2)
            {
                var data = arraysPool.Get();

                if(!stream.GetNextChunk(data, out var count))
                {
                    if (!loop)
                    {
                        arraysPool.Return(data);
                        return;
                    }

                    stream.ResetPosition();

                    if (!stream.GetNextChunk(data, out count))
                    {
                        arraysPool.Return(data);
                        return;
                    }
                }

                var buffer = new AudioBuffer
                {
                    Stream = DataStream.Create(data, true, true),
                    AudioBytes = count,
                    Flags = BufferFlags.None
                };

                submitedBuffers.Enqueue(data);
                voice.SubmitSourceBuffer(buffer, null);
            }
        }

        public override void Pause()
        {
            voice?.Stop();
        }

        public override void Play(bool loop)
        {
            this.loop = loop;
            SubmitBuffersIfNeeded();
            voice?.Start();
        }

        public override void Reset()
        {
            voice?.FlushSourceBuffers();
            while(submitedBuffers.Count>0)
            {
                arraysPool.Return(submitedBuffers.Dequeue());
            }

            SubmitBuffersIfNeeded();
        }

        public override void Dispose()
        {
            if (disposed) return;

            voice.BufferEnd -= Voice_BufferEnd;
            soundSettings.ParametersChanged -= SoundSettings_ParametersChanged;

            Pause();
            Reset();
            
            var voiceToDestroy = voice;

            voice = null;
            disposed = true;

            dispatcher.BeginInvoke(() =>
            {
                voiceToDestroy.DestroyVoice();
                voiceToDestroy.Dispose();
            });
        }
    }
}