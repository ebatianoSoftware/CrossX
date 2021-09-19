using CrossX.Abstractions.Async;
using System;
using System.Collections.Generic;

namespace CrossX.Framework.Async
{
    internal sealed class Sequencer : ISequencer
    {
        private readonly List<Sequence> sequences = new List<Sequence>();
        private readonly object objLock = new object();

        public void Run(Sequence sequence)
        {
            var sequenceImpl = (SequenceImpl)sequence;

            if (sequenceImpl.IsRun) throw new InvalidOperationException("Sequence already run!");
            sequenceImpl.IsRun = true;

            lock (objLock)
            {
                sequences.Add(sequence);
            }
        }

        public Sequence Run(IEnumerable<Sequence> sequences)
        {
            var sequence = Sequence.Agregate(sequences);
            var sequenceImpl = (SequenceImpl)sequence;
            sequenceImpl.IsRun = true;

            lock (objLock)
            {
                this.sequences.Add(sequence);
            }
            return sequence;
        }

        public void Update(TimeSpan timeSpan)
        {
            int index = 0;
            while(true)
            {
                SequenceImpl sequence = null;

                lock (objLock)
                {
                    if (index < sequences.Count)
                    {
                        sequence = (SequenceImpl)sequences[index];
                    }
                }

                if (sequence != null)
                {
                    if (sequence.ShouldRemove(timeSpan))
                    {
                        lock (objLock)
                        {
                            sequences.Remove(sequence);
                        }
                        continue;
                    }
                    index++;
                    continue;
                }
                
                break;
            }
        }
    }
}
