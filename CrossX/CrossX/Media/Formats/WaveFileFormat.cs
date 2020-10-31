// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Data;
using System;
using System.IO;

namespace CrossX.Media.Formats
{
    /// <summary>
    /// Loads PCM Wave file format.
    /// </summary>
    public sealed class WaveFileFormat : IRawLoader<RawSound>
    {
        public static readonly WaveFileFormat Instance = new WaveFileFormat();

        public string DefaultExtension => ".wav";

        /// <summary>
        /// Loads <see cref="RawSound"/> from stream.
        /// </summary>
        /// <returns><see cref="RawSound"/> loaded from the <paramref name="stream"/>.</returns>
        /// <param name="stream">Wave file stream.</param>
        public RawSound FromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                string signature = new string(reader.ReadChars(4));

                if (signature != "RIFF")
                {
                    throw new NotSupportedException("Specified stream is not a wave file.");
                }

                //Size of the overall file
                reader.ReadInt32();

                // WAVE File Type Header
                var format = new string(reader.ReadChars(4));

                if (format != "WAVE")
                {
                    throw new NotSupportedException("Specified stream is not a wave file.");
                }

                // 'fmt ' Format chunk marker (Includes trailing null)
                var formatSignature = new string(reader.ReadChars(4));

                if (formatSignature != "fmt ")
                {
                    throw new NotSupportedException("Specified wave file is not supported.");
                }

                //Length of format data as listed above
                reader.ReadInt32();

                //Type of format (1 is PCM)
                reader.ReadInt16();
                
                //Number of Channels
                var channels = reader.ReadInt16();

                //Sample Rate
                var sampleRate = reader.ReadInt32();

                //(Sample Rate * BitsPerSample * Channels) / 8
                reader.ReadInt32();

                //(BitsPerSample * Channels) / 8
                reader.ReadInt16();

                //Bits per sample
                var bitsPerSample = (byte)reader.ReadInt16();

                //"data" chunk header, Marks the beginning of the data section
                var dataSignature = new string(reader.ReadChars(4));

                switch (dataSignature)
                {
                    case "data":
                        if (bitsPerSample != 8 && bitsPerSample != 16) throw new NotSupportedException("Specified wave file is not supported.");
                        break;

                    default:
                        throw new NotSupportedException("Specified wave file is not supported.");
                }

                //Size of the data section
                var dataLength = reader.ReadInt32(); // <========== **The correct data length**
                var audioData = reader.ReadBytes(dataLength);

                return new RawSound(channels, bitsPerSample, sampleRate, audioData);
            }
        }
    }
}
