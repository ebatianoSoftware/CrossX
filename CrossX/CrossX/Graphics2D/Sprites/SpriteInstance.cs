using CrossX.Data.Sprites;
using System;
using System.Linq;

namespace CrossX.Graphics2D.Sprites
{
    public class SpriteInstance
    {
        public event Action<SpriteEvent> SpriteEvent;

        public SpriteDefinition SpriteDefinition { get; }

        public SpriteInstance(SpriteDefinition spriteDefinition)
        {
            SpriteDefinition = spriteDefinition;
        }

        public SpriteFrame CurrentFrame { get; private set; }

        private float currentTime = 0;
        private float sequenceTime = 0;
        public SpriteSequence CurrentSequence { get; private set; }

        public void SetSequence(string name, SequenceChangeMode mode)
        {
            var oldSequence = CurrentSequence;
            CurrentSequence = SpriteDefinition.Sequences.FirstOrDefault(o => o.Name == name);

            sequenceTime = 0;

            if(CurrentSequence != null)
            {
                for(var idx =0; idx < CurrentSequence.Frames.Length; ++idx)
                {
                    sequenceTime += CurrentSequence.Frames[idx].FrameTime;
                }
            }

            switch(mode)
            {
                case SequenceChangeMode.Reset:
                    currentTime = 0;
                    break;

                case SequenceChangeMode.ResetIfDifferent:
                    if (oldSequence != CurrentSequence)
                    {
                        currentTime = 0;
                    }
                    break;
            }

            if(CurrentSequence != null)
            {
                currentTime %= CurrentSequence.Frames.Length;
            }

            UpdateFrame();
        }

        private void UpdateFrame()
        {
            if (CurrentSequence == null || CurrentSequence.Frames.Length == 0)
            {
                CurrentFrame = null;
                return;
            }

            var frame = CurrentSequence.Frames[0];
            var time = frame.FrameTime;

            for (var idx = 1; idx < CurrentSequence.Frames.Length; ++idx)
            {
                if (currentTime >= time)
                {
                    frame = CurrentSequence.Frames[idx];
                    time += frame.FrameTime;
                }
                else
                {
                    break;
                }
            }

            if (frame != CurrentFrame)
            {
                CurrentFrame = frame;
                GenerateEvent();
            }
        }

        private void GenerateEvent()
        {
            var events = CurrentFrame?.Events;

            if (events != null)
            {
                for (var idx = 0; idx < events.Length; ++idx)
                {
                    SpriteEvent?.Invoke(events[idx]);
                }
            }
        }

        private void Update(float time)
        {
            if (sequenceTime == 0) return;

            currentTime += time;
            currentTime %= sequenceTime;

            UpdateFrame();
        }
    }
}
