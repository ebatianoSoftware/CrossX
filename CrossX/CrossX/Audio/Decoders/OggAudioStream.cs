using NAudio.Vorbis;
using NVorbis;
using System;
using System.IO;

namespace CrossX.Audio.Decoders
{
    public class OggAudioStream: IAudioStream, IDisposable
    {
        
        private readonly VorbisReader reader;
        private float[] floatBuffer = new float[0];

        public OggAudioStream(Stream stream)
        {
            reader = new VorbisReader(stream);
        }

        public int Channels => reader.Channels;
        public int SampleRate => reader.SampleRate;
        public int BitRate => 16;

        public bool GetNextChunk(byte[] buffer, out int bytes)
        {
            const int bytesPerSample = 2;

            var bufferSize = buffer.Length / bytesPerSample;

            if (floatBuffer.Length < bufferSize) floatBuffer = new float[bufferSize];
            if ((buffer.Length % bytesPerSample) != 0) throw new InvalidOperationException();

            int count = 0;
            try
            {
                count = reader.ReadSamples(floatBuffer, 0, bufferSize);
            }
            catch { }

            bytes = 0;
            if (reader.IsEndOfStream) return false;
            if (count == 0) return false;

            for (var idx = 0; idx < count; idx++)
            {
                var val = (ushort)((uint)(floatBuffer[idx] * 0x7fff) & 0xffff);

                buffer[idx * 2 + 0] = (byte)(val & 0xff);
                buffer[idx * 2 + 1] = (byte)((val >> 8) & 0xff);
            }

            bytes = count * bytesPerSample;
            return true;
        }

        public void ResetPosition()
        {
            reader.SamplePosition = 0;
        }

        public void Dispose()
        {
            reader?.Dispose();
        }
    }
}
