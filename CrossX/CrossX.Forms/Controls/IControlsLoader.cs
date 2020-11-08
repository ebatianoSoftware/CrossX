using CrossX.Forms.Xml;

namespace CrossX.Forms.Controls
{
    public interface IControlsLoader
    {
        Control Load(XNode node);
    }
}
