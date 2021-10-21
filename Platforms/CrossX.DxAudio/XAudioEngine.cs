using SharpDX.XAudio2;
using System;

namespace CrossX.DxAudio
{
    internal class XAudioEngine: IDisposable
    {
        public XAudio2 XAudio { get; }
        public MasteringVoice MasteringVoice { get; }

        public XAudioEngine()
        {
            XAudio = new XAudio2();
            MasteringVoice = new MasteringVoice(XAudio);
        }

        public void Dispose()
        {
            MasteringVoice.Dispose();
            XAudio.Dispose();
        }
    }
}
