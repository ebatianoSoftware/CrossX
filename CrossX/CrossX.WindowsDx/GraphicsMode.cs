namespace CrossX.WindowsDx
{
    public class GraphicsMode
    {
        public const int CurrentResolution = -1;
        public bool Fullscreen { get; set; } = false;
        public bool AllowResize { get; set; } = false;
        public int Width { get; set; } = 1280;
        public int Height { get; set; } = 720;
    }

}
