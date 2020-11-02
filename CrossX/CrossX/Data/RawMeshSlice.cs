using System.Linq;

namespace CrossX.Data
{
    public class RawMeshSlice
    {
        public string Material { get; }
        public uint[] Indices4 { get; }
        public ushort[] Indices2 { get; }

        public RawMeshSlice(string material, uint[] indices)
        {
            Material = material;
            var is4 = false;
            for(var idx =0; idx < indices.Length; ++idx)
            {
                if(indices[idx] > ushort.MaxValue)
                {
                    is4 = true;
                }
            }

            if(is4)
            {
                Indices4 = indices;
            }
            else
            {
                Indices2 = indices.Select(o => (ushort)o).ToArray();
            }
        }
    }
}
