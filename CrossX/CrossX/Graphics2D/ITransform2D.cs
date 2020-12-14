using System.Numerics;

namespace CrossX.Graphics2D
{
    public interface ITransform2D
    {
        Matrix4x4 Transform { get; }
        void Push(Matrix4x4 transform);
        void Pop();
        void Clear();
    }
}
