using System;
using System.Collections.Generic;

namespace CrossX.Utils
{
    public class ObjectPool<T> where T: class
    {
        private Func<T> objectFactory;
        private Action<T> cleanup;
        private Stack<T> pool = new Stack<T>();

        private readonly List<T> objects = new List<T>();

        public void SetCustomFactory(Func<T> factory)
        {
            objectFactory = factory;
        }

        public void SetCustomCleanup(Action<T> cleanup)
        {
            this.cleanup = cleanup;
        }

        public T Get()
        {
            if (pool.Count > 0) return pool.Pop();
            var @obj = objectFactory != null ? objectFactory() : Activator.CreateInstance<T>();
            objects.Add(obj);
            return obj;
        }

        public void Return(T @object)
        {
            cleanup?.Invoke(@object);
            pool.Push(@object);
        }
    }
}
