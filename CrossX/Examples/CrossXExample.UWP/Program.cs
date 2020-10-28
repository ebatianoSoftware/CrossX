using CrossX.Core;
using CrossX.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace CrossXExample.UWP
{
    internal class App : IApp
    {
        public void Draw(double time)
        {
            
        }

        public void LoadContent()
        {
            
        }

        public void Update(double time)
        {
            
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var runner = new AppRunner<App>();
            runner.Run(0);
        }
    }
}
