namespace CrossX.Data.Sprites
{
    public class SpriteDefinition
    {
        public SpriteSequence[] Sequences { get; }

        public SpriteDefinition(SpriteSequence[] sequences)
        {
            Sequences = sequences;
        }
    }
}
