using CrossX.Graphics;
using CrossX.Graphics2D.Text;
using CrossX.IO;
using CrossX.IoC;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrossX.Forms.Services
{
    internal class FontsContainer : IFontsContainer, IFontsLoader
    {
        private readonly IFilesRepository filesRepository;
        private Dictionary<string, List<Font>> fonts = new Dictionary<string, List<Font>>();
        private string @default;

        private IObjectFactory objectFactory;

        private Dictionary<FontStyle, FontStyle[]> searchPaths = new Dictionary<FontStyle, FontStyle[]>
        {
            {FontStyle.Regular, new [] { FontStyle.Regular} },
            {FontStyle.Bold, new [] { FontStyle.Bold, FontStyle.Regular} },
            {FontStyle.Italic, new [] { FontStyle.Italic, FontStyle.Regular} },
            {FontStyle.BoldItalic, new [] { FontStyle.BoldItalic, FontStyle.Bold, FontStyle.Italic, FontStyle.Regular} },
        };

        public FontsContainer(IObjectFactory objectFactory, IFilesRepository filesRepository)
        {
            this.objectFactory = objectFactory;
            this.filesRepository = filesRepository;
        }

        public Font Find(string face, float size, FontStyle fontStyle)
        {
            if (face == null || !fonts.TryGetValue(face, out var list))
            {
                face = @default;
                if (!fonts.TryGetValue(face, out list)) throw new KeyNotFoundException();
            }

            var styles = searchPaths[fontStyle];

            foreach (var style in styles)
            {
                Font font = null;
                for (var idx = 0; idx < list.Count; ++idx)
                {
                    if (list[idx].FontInfo.FontStyle == style)
                    {
                        font = list[idx];
                        if (font.FontInfo.OriginalPixelHeight > size)
                        {
                            return font;
                        }
                    }
                }
                if (font != null) return font;
            }

            throw new KeyNotFoundException();
        }


        public void LoadFont(string path)
        {
            Font font;
            var dir = Path.GetDirectoryName(path);
            using (var stream = filesRepository.Open(path))
            {
                font = new Font(stream, p =>
                {
                    using (var str = filesRepository.Open(Path.Combine(dir, p)))
                    {
                        return objectFactory.Create<Texture2D>(str);
                    }
                });
            }

            var face = font.FontInfo.Face;
            if (fonts.Count == 0) @default = face;

            if (!fonts.TryGetValue(face, out var list))
            {
                list = new List<Font>();
                fonts.Add(face, list);
            }
            
            list.Add(font);
            list.Sort((e1, e2) => e1.FontInfo.OriginalPixelHeight - e2.FontInfo.OriginalPixelHeight);
        }
    }
}
