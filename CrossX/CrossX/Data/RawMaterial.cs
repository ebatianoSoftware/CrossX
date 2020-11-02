namespace CrossX.Data
{
    public class RawMaterial
    {
        public string Texture { get; }
        public Color4 Ambient { get; }
        public Color4 Diffuse { get; }
        public Color4 Specular { get; }
        public float SpecularExponent { get; }

        public RawMaterial(string texture, Color4 ambient, Color4 diffuse, Color4 specular, float specularExponent)
        {
            Texture = texture;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            SpecularExponent = specularExponent;
        }
    }
}
