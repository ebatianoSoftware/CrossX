using System;
using System.Collections.Generic;

namespace CrossX.Content
{
    public class ContentManager : IContentManager
    {
        private class ContentEntry
        {
            public IDisposable Content { get; }
            public HashSet<object> Consumers { get; } = new HashSet<object>();

            public ContentEntry(IDisposable content)
            {
                Content = content;
            }
        }

        private Dictionary<Type, object> loaders = new Dictionary<Type, object>();
        private Dictionary<string, ContentEntry> contentEntries = new Dictionary<string, ContentEntry>();

        private List<string> removeList = new List<string>();

        public bool CanLoadContent<TContent>() where TContent : class, IDisposable => loaders.ContainsKey(typeof(TContent));

        public TContent Get<TContent>(string path, object consumer) where TContent : class, IDisposable
        {
            // TODO: Proper exception
            if (!CanLoadContent<TContent>()) throw new InvalidProgramException();

            if(!contentEntries.TryGetValue(GetKey<TContent>(path), out var obj))
            {
                var loadObj = ((IContentLoader<TContent>)loaders[typeof(TContent)]).LoadContent(path);
                obj = new ContentEntry(loadObj);
            }

            if (!obj.Consumers.Contains(consumer))
            {
                obj.Consumers.Add(consumer);
            }

            return (TContent)obj.Content;
        }

        public TContent Get<TContent>(string path) where TContent : class, IDisposable
        {
            return Get<TContent>(path, this);
        }

        public void ReleaseContent<TContent>(TContent content, object consumer) where TContent : class, IDisposable
        {
            removeList.Clear();
            foreach (var entry in contentEntries)
            {
                if(entry.Value.Content == content)
                {
                    entry.Value.Consumers.Remove(consumer);
                    if(entry.Value.Consumers.Count == 0)
                    {
                        entry.Value.Content.Dispose();
                        removeList.Add(entry.Key);
                    }
                }
            }

            foreach(var key in removeList)
            {
                contentEntries.Remove(key);
            }
            removeList.Clear();
        }

        public void SetContentLoader<TContent>(IContentLoader<TContent> contentLoader) where TContent: class, IDisposable
        {
            loaders[typeof(TContent)] = contentLoader;
        }

        private string GetKey<TContent>(string path) where TContent : class, IDisposable
        {
            return path + "(" + typeof(TContent).FullName + ")";
        }
    }
}
