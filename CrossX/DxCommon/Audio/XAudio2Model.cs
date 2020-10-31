using SharpDX.X3DAudio;
using System;

namespace CrossX.DxCommon.Audio
{
    static class XAudio2Model
    {
        public static void Calculate(X3DAudio x3dAudio, float volume, DxAudioListener dxListener, DxAudioEmitter dxEmitter, float[] matrix, out float doplerFactor)
        {
            var listener = dxListener.Listener;
            var emitter = dxEmitter.Emitter;

            var vector = emitter.Position.ToVector3() - listener.Position.ToVector3();
            var dist = vector.Length();

            dist = Math.Min(1, Math.Max(((dist - dxEmitter.MinDistance) / (dxEmitter.MaxDistance - dxEmitter.MinDistance)), 0.00001f));
            var vol = Math.Pow(1 - dist, dxEmitter.Rolloff) * volume;

            var dspSettings = x3dAudio.Calculate(listener, emitter, CalculateFlags.Matrix | CalculateFlags.Doppler, 2, 2);

            for (var idx = 0; idx < dspSettings.MatrixCoefficients.Length; ++idx)
            {
                dspSettings.MatrixCoefficients[idx] *= (float)vol;
            }

            matrix[0] = dspSettings.MatrixCoefficients[0];
            matrix[1] = dspSettings.MatrixCoefficients[2];

            doplerFactor = dspSettings.DopplerFactor;
        }

        public static void Calculate2(float volume, DxAudioListener dxListener, DxAudioEmitter dxEmitter, float[] matrix, out float doplerFactor)
        {
            var vector = dxEmitter.Position - dxListener.Position;
            var dist = vector.Length();

            dist = Math.Min(1, Math.Max(((dist - dxEmitter.MinDistance) / (dxEmitter.MaxDistance - dxEmitter.MinDistance)), 0.00001f));
            
            var vol = Math.Pow(1 - dist, dxEmitter.Rolloff) * volume;

            var vec = dxEmitter.Position - dxListener.Position;

            var factor = Vector3.Dot(dxListener.Forward.Normalized(), vec.Normalized());
            vol *= (factor + 1.5f) / 2.5f;

            var pan = vector.Normalized().X;

            var left = 0.5 - pan / 2.2;
            var right = 0.5 + pan / 2.2;

            matrix[0] = (float)(left * vol);
            matrix[1] = (float)(right * vol);

            var newListenerPos = dxListener.Position + dxListener.Velocity;
            var newEmitterPos = dxEmitter.Position + dxEmitter.Velocity;

            var newDist = (newEmitterPos - newListenerPos).Length();

            const float SpeedOfSound = 344.92f;

            doplerFactor = (float)(SpeedOfSound / (SpeedOfSound + (newDist - dist) * dxEmitter.DopplerScale));
        }
    }
}
