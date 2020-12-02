using CrossX.Data;
using System;
using System.IO;

namespace CrossX.Media.Formats.Mesh3D
{
    public class XxStaticMesh : IRawWriter<RawMesh>
    {
        public void ToStream(Stream stream, RawMesh mesh)
        {
            var writer = new BinaryWriter(stream);

            writer.Write((byte)'X');
            writer.Write((byte)'X');
            writer.Write((byte)'S');
            writer.Write((byte)'M');

            writer.Write(mesh.Vertices.Length);

            foreach(var vertex in mesh.Vertices)
            {
                writer.Write(vertex);
            }

            writer.Write(mesh.Slices.Length);
            foreach(var slice in mesh.Slices)
            {
                writer.Write(slice.Material);
                
                if(slice.Indices2 != null)
                {
                    writer.Write((byte)2);
                    var bytes = new byte[slice.Indices2.Length * 2];
                    Buffer.BlockCopy(slice.Indices2, 0, bytes, 0, bytes.Length);
                    writer.Write(bytes);
                }
                else
                {
                    writer.Write((byte)4);
                    var bytes = new byte[slice.Indices2.Length * 4];
                    Buffer.BlockCopy(slice.Indices4, 0, bytes, 0, bytes.Length);
                    writer.Write(bytes);
                }
            }
        }
    }
}
