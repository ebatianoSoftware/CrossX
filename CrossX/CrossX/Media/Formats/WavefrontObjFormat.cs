using CrossX.Data;
using System.IO;
using JeremyAnsel.Media.WavefrontObj;
using CrossX.Graphics;
using System.Collections.Generic;

namespace CrossX.Media.Formats
{
    public class WavefrontObjFormat : IRawLoader<RawMesh>
    {
        public static readonly IRawLoader<RawMesh> Instance = new WavefrontObjFormat();
        public RawMesh FromStream(Stream stream)
        {
            var file = ObjFile.FromStream(stream);
            
            var vertices = new List<VertexPNT>();
            var map = new Dictionary<ObjTriplet, uint>();

            Dictionary<string, List<uint>> objSlices = new Dictionary<string, List<uint>>();

            for (var idx = 0; idx < file.Faces.Count; ++idx)
            {
                var mat = file.Faces[idx].MaterialName ?? "";

                if (!objSlices.TryGetValue(mat, out var list))
                {
                    list = new List<uint>();
                    objSlices.Add(mat, list);
                }

                var faces = file.Faces[idx];
                var p1 = GetIndex(file, faces.Vertices[0], map, vertices);

                for(var vi = 1; vi < faces.Vertices.Count-1; vi++)
                {
                    var p2 = GetIndex(file, faces.Vertices[vi], map, vertices);
                    var p3 = GetIndex(file, faces.Vertices[vi+1], map, vertices);

                    list.Add(p1);
                    list.Add(p2);
                    list.Add(p3);
                }
            }

            var slices = new RawMeshSlice[objSlices.Count];
            var sliceIndex = 0;

            foreach(var sl in objSlices)
            {
                slices[sliceIndex++] = new RawMeshSlice(sl.Key, sl.Value.ToArray());
            }

            return new RawMesh(slices, vertices.ToArray());
        }

        private uint GetIndex(ObjFile file, ObjTriplet tri, Dictionary<ObjTriplet, uint> map, List<VertexPNT> vertices)
        {
            if (!map.TryGetValue(tri, out var index))
            {
                index = (uint)vertices.Count;
                map.Add(tri, index);
                vertices.Add(FromData(file, tri));
            }
            return index;
        }

        private VertexPNT FromData(ObjFile file, ObjTriplet tri)
        {
            var oVert = file.Vertices;
            var oNormals = file.VertexNormals;
            var oTexCoords = file.TextureVertices;

            var normal = oNormals != null ? new Vector4(oNormals[tri.Normal-1].X, oNormals[tri.Normal - 1].Y, oNormals[tri.Normal-1].Z, 0) : Vector4.Zero;
            var texCoord = oTexCoords != null ? new Vector2(oTexCoords[tri.Texture-1].X, oTexCoords[tri.Texture-1].Y) : Vector2.Zero;
            var position = new Vector4(oVert[tri.Vertex-1].Position.X, oVert[tri.Vertex-1].Position.Y, oVert[tri.Vertex-1].Position.Z, oVert[tri.Vertex-1].Position.W);
            
            return new VertexPNT
            {
                Position = position,
                Normal = normal,
                TextureCoordinate = texCoord
            };
        }

        //private Color4 FromObjVector4(ObjVector4? v)
        //{
        //    if (!v.HasValue) return Color4.White;
        //    var col = v.Value;
        //    return new Color4(col.X, col.Y, col.Z, col.W);
        //}
    }
}