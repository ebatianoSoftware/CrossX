using Xx;

namespace CrossX.Framework.Styles
{
    [XxSchemaPattern(@".*")]
    public struct ThemeValueKey
    {
        public readonly string Value;

        public ThemeValueKey(string value) : this()
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static ThemeValueKey Parse(string value) => new ThemeValueKey(value);

        public static readonly string SystemBackgroundColor = nameof(SystemBackgroundColor);
        public static readonly string SystemForegroundColor = nameof(SystemForegroundColor);
        public static readonly string SystemForegroundColorDisabled = nameof(SystemForegroundColorDisabled);

        public static readonly string SystemButtonBackgroundColor = nameof(SystemButtonBackgroundColor);
        public static readonly string SystemButtonBackgroundColorOver = nameof(SystemButtonBackgroundColorOver);
        public static readonly string SystemButtonBackgroundColorPushed = nameof(SystemButtonBackgroundColorPushed);
        public static readonly string SystemButtonBackgroundColorDisabled = nameof(SystemButtonBackgroundColorDisabled);

        public static readonly string SystemButtonForegroundColor = nameof(SystemButtonForegroundColor);
        public static readonly string SystemButtonForegroundColorOver = nameof(SystemButtonForegroundColorOver);
        public static readonly string SystemButtonForegroundColorPushed = nameof(SystemButtonForegroundColorPushed);
        public static readonly string SystemButtonForegroundColorDisabled = nameof(SystemButtonForegroundColorDisabled);

        public static readonly string SystemButtonAccentBackgroundColor = nameof(SystemButtonAccentBackgroundColor);
        public static readonly string SystemButtonAccentBackgroundColorOver = nameof(SystemButtonAccentBackgroundColorOver);
        public static readonly string SystemButtonAccentBackgroundColorPushed = nameof(SystemButtonAccentBackgroundColorPushed);

        public static readonly string SystemTextFontFamily = nameof(SystemTextFontFamily);
        public static readonly string SystemTextFontSize = nameof(SystemTextFontSize);
        public static readonly string SystemTextFontWeight = nameof(SystemTextFontWeight);

        public static readonly string SystemButtonTextFontFamily = nameof(SystemButtonTextFontFamily);
        public static readonly string SystemButtonTextFontSize = nameof(SystemButtonTextFontSize);
        public static readonly string SystemButtonTextFontWeight = nameof(SystemButtonTextFontWeight);

        public static readonly string SystemButtonTextPadding = nameof(SystemButtonTextPadding);

        public static readonly string SystemCheckRadioSize = nameof(SystemCheckRadioSize);
        
    }

    [XxSchemaPattern(@".*")]
    public struct ResourceValueKey
    {
        public readonly string Value;

        public ResourceValueKey(string value) : this()
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static ResourceValueKey Parse(string value) => new ResourceValueKey(value);

        public static readonly string SystemButtonBackgroundDrawable = nameof(SystemButtonBackgroundDrawable);

        public static readonly string SystemCheckBoxDrawable = nameof(SystemCheckBoxDrawable);
        public static readonly string SystemCheckBoxTickDrawable = nameof(SystemCheckBoxTickDrawable);
        public static readonly string SystemRadioDrawable = nameof(SystemRadioDrawable);
        public static readonly string SystemRadioTickDrawable = nameof(SystemRadioTickDrawable);

        public static readonly string SystemSplitterDrawable = nameof(SystemSplitterDrawable);
        public static readonly string SystemSliderThumbDrawable = nameof(SystemSliderThumbDrawable);
    }
}
