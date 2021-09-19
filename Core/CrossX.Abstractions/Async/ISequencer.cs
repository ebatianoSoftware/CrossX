using System.Collections.Generic;

namespace CrossX.Abstractions.Async
{
    public interface ISequencer
    {
        void Run(Sequence sequence);
        Sequence Run(IEnumerable<Sequence> sequence);
    }
}
