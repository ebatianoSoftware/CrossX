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
            if (sequence.IsRun) throw new InvalidOperationException("Sequence already run!");
            sequence.IsRun = true;

            lock (objLock)
            {
                sequences.Add(sequence);
            }
        }

        public Sequence Run(IEnumerable<Sequence> sequences)
        {
            var sequence = Sequence.Agregate(sequences);
            sequence.IsRun = true;

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
                Sequence sequence = null;

                lock (objLock)
                {
                    if (index < sequences.Count)
                    {
                        sequence = sequences[index];
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
