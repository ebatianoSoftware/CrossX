using CrossX.Graphics;

namespace CrossX.Data
{
    public class RawMesh
    {
        public RawMeshSlice[] Slices { get; }
        public VertexPNT[] Vertices { get; }

        public Aabb3 Bounds { get; }

        public RawMesh(RawMeshSlice[] slices, VertexPNT[] vertices)
        {
            Slices = slices;
            Vertices = vertices;

            var max = new Vector3(float.MinValue);
            var min = new Vector3(float.MaxValue);

            for(var idx =0;idx < vertices.Length; ++idx)
            {
                var pos = vertices[idx].Position;

                max.X = MathHelper.Max(max.X, pos.X);
                max.Y = MathHelper.Max(max.Y, pos.Y);
                max.Z = MathHelper.Max(max.Z, pos.Z);

                min.X = MathHelper.Min(min.X, pos.X);
                min.Y = MathHelper.Min(min.Y, pos.Y);
                min.Z = MathHelper.Min(min.Z, pos.Z);
            }

            Bounds = new Aabb3(min, max);
        }
    }
}
