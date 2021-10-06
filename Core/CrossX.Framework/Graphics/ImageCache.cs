using CrossX.Abstractions.IoC;
using CrossX.Framework.Async;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrossX.Framework.Graphics
{
    internal class ImageCache : IImageCache
    {
        private readonly ISystemDispatcher systemDispatcher;
        private readonly IObjectFactory objectFactory;

        private ConcurrentDictionary<string, Image> imageCache = new ConcurrentDictionary<string, Image>();

        private ConcurrentDictionary<string, Task<Image>> runningTasks = new ConcurrentDictionary<string, Task<Image>>();

        public ImageCache(ISystemDispatcher systemDispatcher, IObjectFactory objectFactory)
        {
            this.systemDispatcher = systemDispatcher;
            this.objectFactory = objectFactory;
        }

        public Task<Image> GetImage(string uri)
        {
            if (imageCache.TryGetValue(uri, out var image)) return Task.FromResult(image);

            if (uri.StartsWith("http")) return LoadImageFromWeb(uri);
            return LoadImageFromResource(uri);
        }

        private Task<Image> LoadImageFromWeb(string uri)
        {
            if (runningTasks.TryGetValue(uri, out var runningTask))
            {
                return runningTask;
            }

            var task = Task.Run(async () =>
            {
                var request = WebRequest.Create(uri);

                Stream dataStream = null;

                using (var response = await request.GetResponseAsync())
                {
                    dataStream = new MemoryStream();
                    using (var respStream = response.GetResponseStream())
                    {
                        await respStream.CopyToAsync(dataStream);
                    }
                    dataStream.Seek(0, SeekOrigin.Begin);
                }

                var image = await systemDispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        return objectFactory.Create<Image>(dataStream);
                    }
                    finally
                    {
                        dataStream.Dispose();
                    }
                });

                image = AddImage(uri, image);
                runningTasks.TryRemove(uri, out var _);
                return image;
            });

            if (runningTasks.TryGetValue(uri, out runningTask))
            {
                return runningTask;
            }

            runningTasks.TryAdd(uri, task);
            return task;
        }

        private Task<Image> LoadImageFromResource(string uri)
        {
            return systemDispatcher.InvokeAsync(() =>
            {       
                using (var stream = Utils.OpenEmbededResource(uri))
                {
                    var image = objectFactory.Create<Image>(stream);
                    return AddImage(uri, image);
                }
            });
        }

        private Image AddImage(string uri, Image image)
        {
            if(imageCache.TryGetValue(uri, out var img))
            {
                image.Dispose();
                return img;
            }

            if (!imageCache.TryAdd(uri, image))
            {
                image.Dispose();
                if (imageCache.TryGetValue(uri, out image)) return image;
                throw new InvalidOperationException();
            }
            return image;
        }
    }
}
