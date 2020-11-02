namespace CrossX.Data
{
    public class RawMaterial
    {
        public string Name { get; }
        public string DiffuseMap { get; }
        public string NormalMap { get; }
        public Color4 Ambient { get; }
        public Color4 Diffuse { get; }
        public Color4 Specular { get; }
        public float SpecularExponent { get; }

        public RawMaterial(string name, string diffuseMap, string normalMap, Color4 ambient, Color4 diffuse, Color4 specular, float specularExponent)
        {
            DiffuseMap = diffuseMap;
            NormalMap = normalMap;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            SpecularExponent = specularExponent;
        }
    }
}
