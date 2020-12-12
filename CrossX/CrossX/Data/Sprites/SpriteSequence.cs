namespace CrossX.Data.Sprites
{
    public class SpriteSequence
    {
        public string Name { get; }
        public string SpriteSheet { get; }
        public SpriteFrame[] Frames { get; }

        public SpriteSequence(string name, string spriteSheet, SpriteFrame[] frames)
        {
            Name = name;
            SpriteSheet = spriteSheet;
            Frames = frames;
        }
    }
}