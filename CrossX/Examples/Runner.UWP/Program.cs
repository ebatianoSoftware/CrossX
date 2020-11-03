using CrossX.UWP;
using CrossX.UWP.UWP;
using Windows.UI.ViewManagement;

//using App = T01.SimpleTriangle.T01_SimpleTriangleApp;
//using App = T02.Textures.T02_TexturesApp;
//using App = T03.InputGamepadAndKeyboard.T03_InputGamepadAndKeyboardApp;
//using App = T04.Graphics2D.T04_Graphics2DApp;
//using App = T05.Audio.T05_AudioApp;
using App = T06.StaticMesh.T06_MeshApp;

namespace CrossXExample.UWP
{
    public class Program
    {
        static void Main(string[] _)
        {
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            var runner = new AppRunner<App>();
            runner.Run(0);
        }
    }
}
