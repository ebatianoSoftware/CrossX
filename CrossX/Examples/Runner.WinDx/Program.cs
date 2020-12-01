using CrossX.WindowsDx;
using System;

//using App = T01.SimpleTriangle.T01_SimpleTriangleApp;
//using App = T02.Textures.T02_TexturesApp;
//using App = T03.InputGamepadAndKeyboard.T03_InputGamepadAndKeyboardApp;
//using App = T04.Graphics2D.T04_Graphics2DApp;
//using App = T05.Audio.T05_AudioApp;
using App = T06.StaticMesh.T06_MeshApp;

namespace Runner.WinDx
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var runner = new AppRunner<App>("Test", new GraphicsMode
            {
                AllowResize = false,
                Fullscreen = false,
                Width = 1280,
                Height = 720
            });
            runner.Run();
        }
    }
}
