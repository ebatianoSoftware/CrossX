using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossX.Abstractions.Async
{
    public static class AsyncExtensions
    {
        public static Sequence AsSequence(this Task task)
        {
            return Sequence.Agregate(GetSequence(task, 0));
        }

        public static Sequence AsSequence(this IEnumerable<Sequence> Sequences)
        {
            return Sequence.Agregate(Sequences);
        }

        public static Sequence AsSequence(this Task task, int checkIntervalInMs = 0)
        {
            return Sequence.Agregate(GetSequence(task, checkIntervalInMs));
        }

        private static IEnumerable<Sequence> GetSequence(Task task, int ms)
        {
            while (!task.IsCompleted)
            {
                var seconds = ms / 1000.0;
                yield return ms == 0 ? Sequence.WaitForNextFrame() : Sequence.WaitForSeconds(seconds);
            }

            task.Wait();
        }
    }
}
