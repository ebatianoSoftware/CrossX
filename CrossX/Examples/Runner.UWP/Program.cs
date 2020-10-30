using CrossX.UWP;

//using App = T01.SimpleTriangle.T01_SimpleTriangleApp;
using App = T02.Textures.T02_TexturesApp;
//using App = T03.InputGamepadAndKeyboard.T03_InputGamepadAndKeyboardApp;

namespace CrossXExample.UWP
{
    public class Program
    {
        static void Main(string[] _)
        {
            var runner = new AppRunner<App>();
            runner.Run(0);
        }   
    }
}
