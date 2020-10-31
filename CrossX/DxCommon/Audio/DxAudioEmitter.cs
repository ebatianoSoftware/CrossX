using CrossX.Audio;
using SharpDX.X3DAudio;
using System;
using System.Collections.Generic;

namespace CrossX.DxCommon.Audio
{
    internal class DxAudioEmitter : AudioEmitter
    {
        public override Vector3 Position { get => Emitter.Position.ToVector3(); set => SetProperty(ref Emitter.Position, value.ToRawVector()); }
        public override Vector3 Velocity { get => Emitter.Velocity.ToVector3(); set => SetProperty(ref Emitter.Velocity, value.ToRawVector()); }
        public override Vector3 Up { get => Emitter.OrientTop.ToVector3(); set => SetProperty(ref Emitter.OrientTop, value.ToRawVector()); }
        public override Vector3 Forward { get => Emitter.OrientFront.ToVector3(); set => SetProperty(ref Emitter.OrientFront, value.ToRawVector()); }
        public override float DopplerScale { get => Emitter.DopplerScaler; set => SetProperty(ref Emitter.DopplerScaler, (float)value); }

        public override float MinDistance { get => minDistance; set => SetProperty(ref minDistance, value); }
        public override float MaxDistance { get => maxDistance; set => SetProperty(ref maxDistance, value); }

        public override float Rolloff { get => rolloff; set => SetProperty(ref rolloff, value); }

        public event Action ValuesChanged;

        public readonly Emitter Emitter = new Emitter
        { 
            CurveDistanceScaler = float.MinValue, 
            InnerRadius = 1.0f, 
            InnerRadiusAngle = 0.1f 
        };

        private float minDistance;
        private float maxDistance;
        private float rolloff;

        protected void SetProperty<T>(ref T prop, T value)
        {
            if (EqualityComparer<T>.Default.Equals(prop, value)) return;
            prop = value;
            ValuesChanged?.Invoke();
        }
    }
}