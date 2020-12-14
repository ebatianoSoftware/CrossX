using CrossX.Graphics;
using System.Collections.Generic;
using System.Numerics;

namespace CrossX.Graphics2D
{
    public class Transform2D : ITransform2D
    {
        public Matrix4x4 Transform => stack.Count > 0 ? stack.Peek() : Matrix4x4.Identity;

        private readonly Stack<Matrix4x4> stack = new Stack<Matrix4x4>();
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

        public void Push(Matrix4x4 transform)
        {
            graphicsDevice.Flush(this);
            stack.Push(transform * Transform);
        }
    }
}
