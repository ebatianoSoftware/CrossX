using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossX.Abstractions.Async
{
    public abstract class Sequence
    {
        public delegate Sequence CreateSequenceDelegate(int frames, double seconds, Func<bool> condition, IEnumerator<Sequence> enumerator);

        public static CreateSequenceDelegate CreateSequenceFunc;
        public static Sequence NextFrameSequence;

        public abstract bool IsFinished { get; }
        public abstract bool IsCanceled { get; }

        public abstract Task Task { get; }

        public static Sequence WaitForNextFrame() => NextFrameSequence;
        public static Sequence WaitForFrames(int frames) => CreateSequenceFunc(frames, 0, null, null);
        public static Sequence WaitForSeconds(double seconds) => CreateSequenceFunc(0, seconds, null, null);
        public static Sequence WaitForCondition(Func<bool> func) => CreateSequenceFunc(0, 0, func, null);
        public static Sequence DelayAction(double seconds, Action action) => Agregate(Delay(seconds, action));

        private static IEnumerable<Sequence> Delay(double seconds, Action action)
        {
            yield return WaitForSeconds(seconds);
            action.Invoke();
        }

        public static Sequence Agregate(IEnumerable<Sequence> Sequence)
        {
            var enumerator = Sequence.GetEnumerator();
            if (!enumerator.MoveNext()) return WaitForNextFrame();

            return CreateSequenceFunc(0, 0, null, enumerator);
        }

        public abstract void Cancel();
    }
}
