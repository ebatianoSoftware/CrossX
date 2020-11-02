using CrossX.Graphics;

namespace CrossX.Data
{
    public class RawMesh
    {
        public RawMeshSlice[] Slices { get; }
        public VertexPNCT[] Vertices { get; }
        public RawMesh(RawMeshSlice[] slices, VertexPNCT[] vertices)
        {
            Slices = slices;
            Vertices = vertices;
        }
    }
}
