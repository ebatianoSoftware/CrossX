// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using CrossX.Graphics;
using SharpFNT;
using System.IO;

namespace CrossX.Graphics2D.Text
{
    public delegate Texture2D LoadFontSheetDelegate(string name);

    public sealed class Font
    {
        public Texture2D[] Textures { get; }
        public FontInfo FontInfo { get; }

        public Font(Stream stream, LoadFontSheetDelegate loadFontSheet)
        {
            FontInfo info = LoadFontInfo(stream);
            var textures = new Texture2D[info.Pages.Length];

            for (var idx = 0; idx < textures.Length; ++idx)
            {
                textures[idx] = loadFontSheet(info.Pages[idx]);
            }

            FontInfo = info;
            Textures = textures;
        }

        private FontInfo LoadFontInfo(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            stream.Close();

            try
            {
                var fnt = BitmapFont.FromStream(memoryStream, FormatHint.Binary, false);
                return new FontInfo(fnt);
            }
            catch
            {
                try
                {
                    var fnt = BitmapFont.FromStream(memoryStream, FormatHint.Text, false);
                    return new FontInfo(fnt);
                }
                catch
                {
                    var fnt = BitmapFont.FromStream(memoryStream, FormatHint.XML, false);
                    return new FontInfo(fnt);
                }
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var texture in Textures) texture.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Font() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
