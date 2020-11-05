using System;

namespace CrossX.Content
{
    public interface IContentManager
    {
        TContent Get<TContent>(string path, object consumer) where TContent : class, IDisposable;
        TContent Get<TContent>(string path) where TContent : class, IDisposable;
        void ReleaseContent<TContent>(TContent content, object consumer) where TContent : class, IDisposable;
        void SetContentLoader<TContent>(IContentLoader<TContent> contentLoader) where TContent : class, IDisposable;
        bool CanLoadContent<TContent>() where TContent : class, IDisposable;
    }
}
