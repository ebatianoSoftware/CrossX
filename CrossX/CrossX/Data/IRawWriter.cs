using System.IO;

namespace CrossX.Data
{
    public interface IRawWriter<T>
    {
        void ToStream(Stream stream, T obj);
    }
}
