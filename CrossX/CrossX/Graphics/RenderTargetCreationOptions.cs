namespace CrossX.Graphics
{
    public sealed class RenderTargetCreationOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public RenderTargetContent Content { get; set; } = RenderTargetContent.Both;
    }
}
