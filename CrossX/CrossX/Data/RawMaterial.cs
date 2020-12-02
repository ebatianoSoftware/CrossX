using System;
using System.IO;

namespace CrossX.Data
{
    public class RawMaterial: IEquatable<RawMaterial>
    {
        public string Name { get; }
        public string DiffuseMap { get; }
        public string NormalMap { get; }
        public string SpecularMap { get; }
        public Color4 Ambient { get; }
        public Color4 Diffuse { get; }
        public Color4 Specular { get; }
        public Color4 Emissive { get; }
        public Color4 Glow { get; }
        public float SpecularExponent { get; }

        public RawMaterial(string name, string diffuseMap, string normalMap, string specularMap, Color4 ambient, Color4 diffuse, Color4 specular, Color4 emissive, Color4 glow, float specularExponent)
        {
            DiffuseMap = diffuseMap;
            Emissive = emissive;
            Glow = glow;
            NormalMap = normalMap;
            Ambient = ambient;
            Diffuse = diffuse;
            
            SpecularMap = specularMap;
            Specular = specular;
            SpecularExponent = specularExponent;
        }

        public bool Equals(RawMaterial other)
        {
            return other.Diffuse == Diffuse &&
                   other.Ambient == Ambient &&
                   other.Specular == Specular &&
                   other.SpecularExponent == SpecularExponent &&
                   DiffuseMap == DiffuseMap &&
                   NormalMap == NormalMap &&
                   SpecularMap == SpecularMap;
        }

        public override bool Equals(object obj)
        {
            if (obj is RawMaterial rm)
            {
                return rm.Equals(this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Diffuse.GetHashCode() ^ Ambient.GetHashCode() ^ Specular.GetHashCode() ^ SpecularExponent.GetHashCode() ^
                   NormalMap?.GetHashCode() ?? 0 ^ SpecularMap?.GetHashCode() ?? 0 ^ DiffuseMap?.GetHashCode() ?? 0;
        }
    }
}
