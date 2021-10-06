using System.Runtime.CompilerServices;

namespace CrossX.Framework.Graphics
{
    public interface IRedrawService
    {
        void RequestRedraw([CallerMemberName] string caller = "", [CallerFilePath] string path = "");
    }
}
