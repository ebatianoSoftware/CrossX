using System.Drawing;

namespace CrossX.WindowsDx
{
    public class GraphicsMode
    {
        public const int CurrentResolution = -1;
        public bool Fullscreen { get; set; } = false;
        public bool AllowResize { get; set; } = false;
        public Size WindowSize { get; set; }
    }

}
