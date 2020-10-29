using System;
using System.Collections.Generic;

namespace CrossX.Async
{
    public class CoroutineService : ICoroutineService
    {
        private readonly List<Coroutine> _coroutines = new List<Coroutine>();

        public void Run(Coroutine coroutine)
        {
            _coroutines.Add(coroutine);
        }

        public void Run(IEnumerable<Coroutine> coroutine)
        {
            _coroutines.Add(Coroutine.Agregate(coroutine));
        }

        public void Update(TimeSpan timeSpan)
        {
            for(var idx = 0; idx < _coroutines.Count;)
            {
                var coroutine = _coroutines[idx];

                if(coroutine.ShouldRemove(timeSpan))
                {
                    _coroutines.RemoveAt(idx);
                    continue;
                }
                idx++;
            }
        }
    }
}
