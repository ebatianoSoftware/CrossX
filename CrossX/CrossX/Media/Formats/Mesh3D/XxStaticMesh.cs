using CrossX.Data;
using CrossX.Graphics;
using System;
using System.IO;

namespace CrossX.Media.Formats.Mesh3D
{
    public class XxStaticMesh : IRawWriter<RawMesh>, IRawLoader<RawMesh>
    {
        public static readonly XxStaticMesh Instance = new XxStaticMesh();

        public RawMesh FromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var bytes = reader.ReadBytes(4);
            if (bytes[0] != 'X' && bytes[1] != 'X' && bytes[2] != 'S' && bytes[3] != 'M') throw new InvalidDataException();

            var vertCount = reader.ReadInt32();
            VertexPNT[] vertices = new VertexPNT[vertCount];

            for(var idx =0; idx < vertCount; ++idx)
            {
                vertices[idx] = reader.ReadVertexPNT();
            }

            var sliceCount = reader.ReadInt32();
            var slices = new RawMeshSlice[sliceCount];

            for (var idx = 0; idx < sliceCount; ++idx)
            {
                var material = reader.ReadRawMaterial();
                var kind = reader.ReadByte();
                var count = reader.ReadInt32();

                ushort[] indices2 = null;
                uint[] indices4 = null;

                switch (kind)
                {
                    case 2:
                        bytes = reader.ReadBytes(count * 2);
                        indices2 = new ushort[count];
                        Buffer.BlockCopy(bytes, 0, indices2, 0, bytes.Length);
                        break;

                    case 4:
                        bytes = reader.ReadBytes(count * 4);
                        indices4 = new uint[count];
                        Buffer.BlockCopy(bytes, 0, indices4, 0, bytes.Length);
                        break;

                    default:
                        throw new InvalidDataException();
                }

                slices[idx] = new RawMeshSlice(material, indices2, indices4);
            }

            return new RawMesh(slices, vertices);
        }

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
                    writer.Write(slice.Indices2.Length);
                    var bytes = new byte[slice.Indices2.Length * 2];
                    Buffer.BlockCopy(slice.Indices2, 0, bytes, 0, bytes.Length);
                    writer.Write(bytes);
                }
                else
                {
                    writer.Write((byte)4);
                    writer.Write(slice.Indices4.Length);
                    var bytes = new byte[slice.Indices2.Length * 4];
                    Buffer.BlockCopy(slice.Indices4, 0, bytes, 0, bytes.Length);
                    writer.Write(bytes);
                }
            }
        }
    }
}
