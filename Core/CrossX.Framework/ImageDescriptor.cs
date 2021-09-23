using CrossX.Abstractions.Mvvm;
using CrossX.Framework.Graphics;
using System;
using System.Collections.Generic;
using Xx;

namespace CrossX.Framework
{
    [XxSchemaPattern(@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?")]
    public struct ImageDescriptor : IEquatable<ImageDescriptor>
    {
        [ValueConverter(typeof(string), typeof(ImageDescriptor))]
        internal class StringToDescriptorConverter : IValueConverter
        {
            public object Convert(object value)
            {
                return new ImageDescriptor((string)value);
            }
        }

        [ValueConverter(typeof(Image), typeof(ImageDescriptor))]
        internal class ImageToDescriptorConverter : IValueConverter
        {
            public object Convert(object value)
            {
                return new ImageDescriptor((Image)value);
            }
        }

        public string Uri { get; }
        public Image Image { get; }

        public ImageDescriptor(string uri): this()
        {
            Uri = uri;
        }

        public ImageDescriptor(Image image) : this()
        {
            Image = image;
        }

        public ImageDescriptor(string uri, Image image) : this()
        {
            Uri = uri;
            Image = image;
        }

        public static implicit operator ImageDescriptor(string uri) => new ImageDescriptor(uri);
        public static implicit operator ImageDescriptor(Image img) => new ImageDescriptor(img);

        public static ImageDescriptor Parse(string text) => new ImageDescriptor(text);

        public override bool Equals(object obj)
        {
            return obj is ImageDescriptor descriptor && Equals(descriptor);
        }

        public bool Equals(ImageDescriptor other)
        {
            return Uri == other.Uri &&
                   EqualityComparer<Image>.Default.Equals(Image, other.Image);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Uri, Image);
        }

        public static bool operator ==(ImageDescriptor d1, ImageDescriptor d2) => d1.Equals(d2);
        public static bool operator !=(ImageDescriptor d1, ImageDescriptor d2) => !d1.Equals(d2);
    }
}
