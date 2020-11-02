using CrossX.Data;
using System.IO;
using JeremyAnsel.Media.WavefrontObj;
using CrossX.Graphics;
using System.Collections.Generic;
using System;

namespace CrossX.Media.Formats
{
    public class WavefrontObjFormat : IRawLoader<RawMesh>
    {
        public RawMesh FromStream(Stream stream)
        {
            var file = ObjFile.FromStream(stream);

            var vertices = new List<VertexPNCT>();
            var map = new Dictionary<ObjTriplet, uint>();

            List<Tuple<string, List<uint>>> faces = new List<Tuple<string, List<uint>>>();

            for(var idx = 0; idx <  file.Faces.Count; ++idx)
            {
                var mat = file.Faces[idx].MaterialName;

                var list = new List<uint>();
                var tuple = Tuple.Create(mat, list);
                faces.Add(tuple);

                foreach (var tri in file.Faces[idx].Vertices)
                {
                    if(!map.TryGetValue(tri, out var index))
                    {
                        map.Add(tri, (uint)vertices.Count);
                        vertices.Add(FromData(file, tri));
                    }
                    list.Add(index);
                }
            }

            var slices = new RawMeshSlice[faces.Count];

            for(var idx =0; idx < slices.Length; ++idx)
            {
                slices[idx] = new RawMeshSlice(faces[idx].Item1, faces[idx].Item2.ToArray());
            }

            return new RawMesh(slices, vertices.ToArray());
        }

        private VertexPNCT FromData(ObjFile file, ObjTriplet tri)
        {
            var oVert = file.Vertices;
            var oNormals = file.VertexNormals;
            var oTexCoords = file.TextureVertices;

            var normal = oNormals != null ? new Vector4(oNormals[tri.Normal].X, oNormals[tri.Normal].Y, oNormals[tri.Normal].Z, 0) : Vector4.Zero;
            var texCoord = oTexCoords != null ? new Vector2(oTexCoords[tri.Texture].X, oTexCoords[tri.Texture].Y) : Vector2.Zero;
            var position = new Vector4(oVert[tri.Vertex].Position.X, oVert[tri.Vertex].Position.Y, oVert[tri.Vertex].Position.Z, oVert[tri.Vertex].Position.W);
            var color = FromObjVector4(oVert[tri.Vertex].Color);

            return new VertexPNCT
            {
                Position = position,
                Normal = normal,
                Color = color,
                TextureCoordinate = texCoord
            };
        }

        private Color4 FromObjVector4(ObjVector4? v)
        {
            if (!v.HasValue) return Color4.White;
            var col = v.Value;
            return new Color4(col.X, col.Y, col.Z, col.W);
        }
    }
}