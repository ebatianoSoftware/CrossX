using System;

namespace CrossX.Content
{
    public interface IContentLoader<TContent> where TContent: class, IDisposable
    {
        TContent LoadContent(string path);
    }

}