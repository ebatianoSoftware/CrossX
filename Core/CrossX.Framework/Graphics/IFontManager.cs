using System.IO;

namespace CrossX.Framework.Graphics
{
    public interface IFontManager
    {
        Font FindFont(string familyName, float fontSize, FontWeight fontWeight, bool italic);
        void LoadTTF(Stream stream, string name = null);
    }
}
