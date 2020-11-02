namespace CrossX.Data
{
    public class RawMeshSlice
    {
        public string Material { get; }
        public ushort[] Indices { get; }

        public RawMeshSlice(string material, ushort[] indices)
        {
            Material = material;
            Indices = indices;
        }
    }
}
