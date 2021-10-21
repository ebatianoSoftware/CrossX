using CrossX.Audio.Sound;
using SharpDX.XAudio2;

namespace CrossX.DxAudio.Sound
{
    internal class DxSoundClipInstance : SoundClipInstance
    {
        public SourceVoice SourceVoice { get; }
        public override float Volume
        {
            get => volume;
            set
            {
                volume = value;
                UpdateVolumes();
            }
        }

        public override float Pan
        {
            get => pan;

            set
            {
                pan = value;
                UpdateVolumes();
            }
        }

        private float pan = 0;
        private float volume = 1;
        private float[] volumes;
        private readonly DxSoundClip dxSoundClip;

        public DxSoundClipInstance(DxSoundClip dxSoundClip, XAudioEngine audioEngine)
        {
            SourceVoice = new SourceVoice(audioEngine.XAudio, dxSoundClip.WaveFormat, true);
            SourceVoice.SubmitSourceBuffer(dxSoundClip.Buffer, dxSoundClip.DecodedPacketsInfo);
            SourceVoice.StreamEnd += SourceVoice_StreamEnd;
            this.dxSoundClip = dxSoundClip;

            volumes = new float[dxSoundClip.WaveFormat.Channels * 2];
            UpdateVolumes();
        }

        private void SourceVoice_StreamEnd()
        {
            if (Loop)
            {
                SourceVoice.SubmitSourceBuffer(dxSoundClip.Buffer, dxSoundClip.DecodedPacketsInfo);
                SourceVoice.Start();
            }
        }

        public override void Play()
        {
            SourceVoice.FlushSourceBuffers();
            SourceVoice.SubmitSourceBuffer(dxSoundClip.Buffer, dxSoundClip.DecodedPacketsInfo);
            UpdateVolumes();
            SourceVoice.Start();
        }

        public override void Stop()
        {
            SourceVoice.Stop();
            SourceVoice.FlushSourceBuffers();
        }

        public override void Pause()
        {
            SourceVoice.Stop();
        }

        public override void Resume()
        {
            SourceVoice.Start();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(disposing && !SourceVoice.IsDisposed)
            {
                SourceVoice.Stop();
                SourceVoice.Dispose();
            }
        }

        private void UpdateVolumes()
        {
            volumes[0] = (1 - pan) / 2;
            volumes[(dxSoundClip.WaveFormat.Channels-1) * 2 + 1] = (1 + pan) / 2;
            SourceVoice.SetVolume(volume);
            SourceVoice.SetOutputMatrix(dxSoundClip.WaveFormat.Channels, 2, volumes);
        }
    }
}