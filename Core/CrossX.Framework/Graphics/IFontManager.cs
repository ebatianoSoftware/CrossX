namespace CrossX.Framework.Graphics
{
    public interface IFontManager
    {
        Font FindFont(string familyName, float fontSize, FontWeight fontWeight, bool italic);
    }
}
