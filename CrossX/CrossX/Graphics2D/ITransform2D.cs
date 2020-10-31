namespace CrossX.Graphics2D
{
    public interface ITransform2D
    {
        Matrix Transform { get; }
        void Push(Matrix transform);
        void Pop();
        void Clear();
    }
}
