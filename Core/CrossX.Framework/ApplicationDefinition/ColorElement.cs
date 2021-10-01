﻿using CrossX.Framework.Styles;
using Xx;

namespace CrossX.Framework.ApplicationDefinition
{
    public interface IValueElement
    {
        ThemeValueKey Key { get; }
        object Value { get; }
    }

    [XxSchemaExport]
    public class ColorElement: IValueElement
    {
        public ThemeValueKey Key { get; set; }

        [XxSchemaBindable]
        public Color Value { get; set; }

        object IValueElement.Value => Value;
    }
}
