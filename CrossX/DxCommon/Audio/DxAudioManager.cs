
using SharpDX.Multimedia;
using SharpDX.X3DAudio;
using SharpDX.XAudio2;
using System;

namespace CrossX.DxCommon.Audio
{
    internal class DxAudioManager: IDisposable
    {
        public readonly MasteringVoice MasteringVoice;
        public readonly X3DAudio X3dAudio;
        public readonly XAudio2 Xaudio2;
        private bool disposed;

        public DxAudioManager()
        {
            Xaudio2 = new XAudio2(XAudio2Version.Default);
            Xaudio2.StartEngine();

            MasteringVoice = new MasteringVoice(Xaudio2);
            MasteringVoice.GetChannelMask(out var channelMask);
            X3dAudio = new X3DAudio((Speakers)channelMask);
        }

        public void Dispose()
        {
            if (disposed) return;
            MasteringVoice.Dispose();
            Xaudio2.Dispose();
            disposed = true;
        }
    }
}