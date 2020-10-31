namespace CrossX.Graphics.Shaders
{
    public abstract class BasicShader
    {
        protected Matrix viewProjectionMatrix;
        protected Matrix worldMatrix;

        public float Alpha { get; set; } = 1;
        public Color4 DiffuseColor { get; set; } = Color4.White;
        public Texture2D Texture { get; set; }
        public void SetWorldTransform(Matrix transform) => worldMatrix = transform;
        public void SetViewProjectionTransform(Matrix transform) => viewProjectionMatrix = transform;
        public abstract SamplerState SamplerState { get;  set; }

        protected BasicShader()
        {
            viewProjectionMatrix = Matrix.Identity;
            worldMatrix = Matrix.Identity;
        }

        public abstract void Apply();
    }
}
