namespace CrossX.Data
{
    public class RawMeshSlice
    {
        public string Material { get; }
        public uint[] Indices { get; }

        public RawMeshSlice(string material, uint[] indices)
        {
            Material = material;
            Indices = indices;
        }
    }
}
