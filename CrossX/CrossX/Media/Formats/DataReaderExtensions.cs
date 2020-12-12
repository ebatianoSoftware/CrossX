using CrossX.Data;
using CrossX.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrossX.Media.Formats
{
    public static class DataReaderExtensions
    {
        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            return new Vector4(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
                );
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
                );
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(
                reader.ReadSingle(),
                reader.ReadSingle()
                );
        }

        public static Color4 ReadColor4(this BinaryReader reader)
        {
            return new Color4(
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte()
                );
        }

        public static VertexPNT ReadVertexPNT(this BinaryReader reader)
        {
            return new VertexPNT
            {
                Position = reader.ReadVector4(),
                Normal = reader.ReadVector4(),
                TextureCoordinate = reader.ReadVector2()
            };
        }

        public static RawMaterial ReadRawMaterial(this BinaryReader reader)
        {

            var diffMap = reader.ReadString();
            var normalMap = reader.ReadString();
            var specularMap = reader.ReadString();

            var specExp = reader.ReadSingle();
            var ambient = reader.ReadColor4();
            var diffuse = reader.ReadColor4();
            var specular = reader.ReadColor4();
            var emissive = reader.ReadColor4();

            return new RawMaterial("", diffMap, normalMap, specularMap, ambient, diffuse, specular, emissive, Color4.Black, specExp);            
        }
    }
}
