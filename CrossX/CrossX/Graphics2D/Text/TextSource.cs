using System;
using System.Text;

namespace CrossX.Graphics2D.Text
{
    public struct TextSource: IEquatable<TextSource>
    {
        private string @string;
        private StringBuilder builder;

        public static implicit operator TextSource(string text) => new TextSource(text);
        public static implicit operator TextSource(StringBuilder text) => new TextSource(text);

        public char this[int index]
        {
            get
            {
                return @string?[index] ?? builder?[index] ?? ' ';
            }
        }

        public int Length => @string?.Length ?? builder?.Length ?? 0;

        public TextSource(string text)
        {
            @string = text;
            builder = null;
        }

        public TextSource(StringBuilder text)
        {
            @string = null;
            builder = text;
        }

        public bool Equals(TextSource other)
        {
            if(other.@string != null && @string != null)
            {
                return @string == other.@string;
            }

            if(other.builder != null && builder != null)
            {
                //if (other.builder.Length != builder.Length) return false;
                //for(var idx =0; idx < builder.Length; ++idx)
                //{
                //    if (builder[idx] != other.builder[idx]) return false;
                //}
                //return true;
                return false;
            }

            return false;
        }
    }
}
