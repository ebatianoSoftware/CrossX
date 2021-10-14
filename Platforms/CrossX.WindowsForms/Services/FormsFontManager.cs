using CrossX.Skia.Graphics;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace CrossX.WindowsForms.Services
{
    public class FormsFontManager : SkiaFontManager
    {
        private PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        public override void LoadTTF(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                base.LoadTTF(memoryStream);
                var bytes = memoryStream.GetBuffer();

                IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(bytes.Length);
                System.Runtime.InteropServices.Marshal.Copy(bytes, 0, fontPtr, bytes.Length);
                privateFontCollection.AddMemoryFont(fontPtr, bytes.Length);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            }

            
        }

        public FontFamily FindFontFamily(string name) => privateFontCollection.Families.FirstOrDefault(f => f.Name == name);

        public Font CreateFont(string fontFamily, float size, GraphicsUnit unit = GraphicsUnit.Point)
        {
            var family = FindFontFamily(fontFamily);

            if (family != null)
            {
                return new Font(family, size, unit);
            }
            else
            {
                return new Font(fontFamily, size, unit);
            }
        }
    }
}
