using CrossX.Data;
using CrossX.Graphics;
using System.IO;
using System.Numerics;

namespace CrossX.Media.Formats
{
    public static class DataWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Vector2 vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
        }

        public static void Write(this BinaryWriter writer, Vector3 vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }

        public static void Write(this BinaryWriter writer, Vector4 vec)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
            writer.Write(vec.W);
        }

        public static void Write(this BinaryWriter writer, Color4 col)
        {
            writer.Write(col.R);
            writer.Write(col.G);
            writer.Write(col.B);
            writer.Write(col.A);
        }

        public static void Write(this BinaryWriter writer, VertexPNT vertex)
        {
            writer.Write(vertex.Position);
            writer.Write(vertex.Normal);
            writer.Write(vertex.TextureCoordinate);
        }

        public static void Write(this BinaryWriter writer, RawMaterial material)
        {
            writer.Write(material.DiffuseMap ?? "");
            writer.Write(material.NormalMap ?? "");
            writer.Write(material.SpecularMap ?? "");
            writer.Write(material.SpecularExponent);

            writer.Write(material.Ambient);
            writer.Write(material.Diffuse);
            writer.Write(material.Specular);
            writer.Write(material.Emissive);
        }
    }
}
