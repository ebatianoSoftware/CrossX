using System;

namespace CrossX.Forms.Attributes
{
    public class ViewAttribute: Attribute
    {
        public ViewAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
