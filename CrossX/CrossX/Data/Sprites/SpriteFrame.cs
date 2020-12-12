using System.Drawing;

namespace CrossX.Data.Sprites
{
    public class SpriteFrame
    {
        public Rectangle SourceRect { get; }
        public Point Origin { get; }
        public float FrameTime { get; }
        public SpriteEvent[] Events { get; }
        
        public SpriteFrame(Rectangle sourceRect, Point origin, float frameTime, SpriteEvent[] events)
        {
            SourceRect = sourceRect;
            Origin = origin;
            FrameTime = frameTime;
            Events = events;
        }
    }
}