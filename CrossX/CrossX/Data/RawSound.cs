// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace CrossX.Data
{
    /// <summary>
    /// Raw sound class.
    /// </summary>
    public sealed class RawSound
    {
        /// <summary>
        /// Number of bits per sample.
        /// </summary>
        public readonly int BitsPerSample;

        /// <summary>
        /// Number of channels.
        /// </summary>
        public readonly int Channels;

        /// <summary>
        /// Sample rate for the sound.
        /// </summary>
        public readonly int SampleRate;

        /// <summary>
        /// Data buffer.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EbatianoSoftware.CrossX.RawMedia.RawSound"/> class.
        /// </summary>
        /// <param name="channels">Number of channels.</param>
        /// <param name="bitsPerSample">Bits per sample.</param>
        /// <param name="sampleRate">Sample rate.</param>
        /// <param name="data">Data.</param>
        public RawSound(int channels, int bitsPerSample, int sampleRate, byte[] data)
        {
            BitsPerSample = bitsPerSample;
            Channels = channels;
            SampleRate = sampleRate;
            Data = data;
        }

        /// <summary>
        /// Converts 16-bit sound to 8 bit sound.
        /// </summary>
        /// <returns>New 8-bit <see cref="RawSound"/>.</returns>
        public RawSound Convert16To8Bits()
        {
            var data = new byte[Data.Length / 2];

            for (var idx = 0; idx < data.Length; ++idx)
            {
                var val = (Data[idx * 2 + 1] | (Data[idx * 2 + 0] << 8)) / (double) (ushort.MaxValue + 1);
                data[idx] = (byte)(val*256);
            }

            return new RawSound(Channels, 8, SampleRate / 2, data);
        }

        /// <summary>
        /// Converts 8-bit sound to 16-bit sound.
        /// </summary>
        /// <returns>New 16-bits <see cref="RawSound"/>.</returns>
        public RawSound Convert8To16Bits()
        {
            var data = new byte[Data.Length * 2];

            for (var idx = 0; idx < Data.Length; ++idx)
            {
                data[idx*2+1] = Data[idx];
            }

            return new RawSound(Channels, 16, SampleRate * 2, data);
        }
    }
}
