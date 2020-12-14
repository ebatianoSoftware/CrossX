using System;
using System.Collections.Generic;
using System.Numerics;
using CrossX.Audio;
using SharpDX.Mathematics.Interop;
using SharpDX.X3DAudio;

namespace CrossX.DxCommon.Audio
{
    static class DxConvert
    {
        public static RawVector3 ToRawVector(this Vector3 point) => new RawVector3(point.X, point.Y, point.Z);
        public static Vector3 ToVector3(this RawVector3 vector) => new Vector3(vector.X, vector.Y, vector.Z);
    }

    internal class DxAudioListener : AudioListener
    {
        public override Vector3 Position { get => Listener.Position.ToVector3(); set => SetProperty(ref Listener.Position, value.ToRawVector()); }
        public override Vector3 Velocity { get => Listener.Velocity.ToVector3(); set => SetProperty(ref Listener.Velocity, value.ToRawVector()); }
        public override Vector3 Up { get => Listener.OrientTop.ToVector3(); set => SetProperty(ref Listener.OrientTop, value.ToRawVector()); }
        public override Vector3 Forward { get => Listener.OrientFront.ToVector3(); set => SetProperty(ref Listener.OrientFront, value.ToRawVector()); }

        public override event Action ValuesChanged;

        public readonly Listener Listener = new Listener
        {
            OrientTop = new RawVector3(0,1,0),
            OrientFront = new RawVector3(0,0,1)
        };

        protected void SetProperty<T>(ref T prop, T value)
        {
            if (EqualityComparer<T>.Default.Equals(prop, value)) return;
            prop = value;
            ValuesChanged?.Invoke();
        }
    }
}