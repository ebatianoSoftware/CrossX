using CrossX.Graphics;
using System.Collections.Generic;

namespace CrossX.Graphics2D
{
    public class Transform2D : ITransform2D
    {
        public Matrix Transform => stack.Count > 0 ? stack.Peek() : Matrix.Identity;

        private readonly Stack<Matrix> stack = new Stack<Matrix>();
        private readonly IGraphicsDevice graphicsDevice;

        public Transform2D(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Clear()
        {
            graphicsDevice.Flush(this);
            stack.Clear();
        }

        public void Pop()
        {
            graphicsDevice.Flush(this);
            stack.Pop();
        }

        public void Push(Matrix transform)
        {
            graphicsDevice.Flush(this);
            stack.Push(transform * Transform);
        }
    }
}
