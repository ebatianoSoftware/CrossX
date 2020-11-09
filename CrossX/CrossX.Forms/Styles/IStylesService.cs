using CrossX.Forms.Xml;

namespace CrossX.Forms.Styles
{
    public interface IStylesService
    {
        void LoadStyles(string path);
    }

    internal interface IStylesServiceEx: IStylesService
    {
        void ApplyStyle(XNode node);
    }
}
