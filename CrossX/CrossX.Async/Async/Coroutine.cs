using System;
using System.Collections.Generic;

namespace CrossX.Async
{
    public class Coroutine
    {
        private static readonly Coroutine NextFrameCoroutine = new Coroutine { _frames = 0 };

        private int _frames;
        private double _seconds;
        private IEnumerator<Coroutine> _enumerator;
        private Func<bool> _condition;

        private Coroutine() { }

        public static Coroutine WaitForNextFrame() => NextFrameCoroutine;
        public static Coroutine WaitForFrames(int frames) => new Coroutine { _frames = frames };
        public static Coroutine WaitForSeconds(double seconds) => new Coroutine { _seconds = seconds };
        public static Coroutine WaitForCondition(Func<bool> func) => new Coroutine{ _condition = func };

        public static Coroutine DelayAction(double seconds, Action action) => Agregate(Delay(seconds, action));

        private static IEnumerable<Coroutine> Delay(double seconds, Action action)
        {
            yield return WaitForSeconds(seconds);
            action.Invoke();
        }

        public static Coroutine Agregate(IEnumerable<Coroutine> coroutine)
        {
            var enumerator = coroutine.GetEnumerator();
            if (!enumerator.MoveNext()) return WaitForNextFrame();

            return new Coroutine { _enumerator = enumerator };
        }

        internal bool ShouldRemove(TimeSpan timeSpan)
        {
            if (_frames > 0) _frames--;
            if (_seconds > 0) _seconds -= timeSpan.TotalSeconds;

            if(_enumerator != null)
            {
                if(_enumerator.Current.ShouldRemove(timeSpan))
                {
                    if(!_enumerator.MoveNext())
                    {
                        _enumerator = null;
                    }
                }
            }

            if (_condition != null )
            {
                if(_condition())
                {
                    _condition = null;
                }
            }

            return _seconds <= 0 && _frames <= 0 && _enumerator == null && _condition == null;
        }
    }
}
