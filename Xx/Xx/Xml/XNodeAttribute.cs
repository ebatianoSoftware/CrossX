// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Globalization;

namespace Xx.Xml
{
    public struct XNodeAttributes
    {
        private readonly XNode node;
        public XNodeAttributes(XNode node)
        {
            this.node = node;
        }

        public bool HasAttribute(string name)
        {
            return node.HasAttribute(name);
        }

        public int AsInt32(string name, int defaultValue)
        {
            return int.TryParse(node.Attribute(name), out var value) ? value : defaultValue;
        }

        public double AsDouble(string name, double defaultValue)
        {
            return double.TryParse(node.Attribute(name), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : defaultValue;
        }

        public decimal AsDecimal(string name, decimal defaultValue)
        {
            return decimal.TryParse(node.Attribute(name), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : defaultValue;
        }

        public string AsString(string name, string defaultValue = "")
        {
            return HasAttribute(name) ? node.Attribute(name) : defaultValue;
        }
        public Guid AsGuid(string name)
        {
            var guidStr = node.Attribute(name) ?? "";

            if (!Guid.TryParse(guidStr, out var guid)) return Guid.NewGuid();
            return guid;
        }

        public bool AsBoolean(string name, bool defaultValue = false)
        {
            return bool.TryParse(node.Attribute(name), out var value) ? value : defaultValue;
        }

        public bool Parse2Int(string name, out int v1, out int v2)
        {
            v1 = default;
            v2 = default;

            var text = node.Attribute(name);

            if (text == null) return false;

            var parts = text.Split(',');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0].Trim(), out v1)) return false;
            if (!int.TryParse(parts[1].Trim(), out v2)) return false;
            return true;
        }

        public bool Parse4Int(string name, out int v1, out int v2, out int v3, out int v4)
        {
            v1 = default;
            v2 = default;
            v3 = default;
            v4 = default;

            var text = node.Attribute(name);

            if (text == null) return false;

            var parts = text.Split(',');
            if (parts.Length != 4) return false;

            if (!int.TryParse(parts[0].Trim(), out v1)) return false;
            if (!int.TryParse(parts[1].Trim(), out v2)) return false;
            if (!int.TryParse(parts[2].Trim(), out v3)) return false;
            if (!int.TryParse(parts[3].Trim(), out v4)) return false;

            return true;
        }

        public bool Parse2Double(string name, out double v1, out double v2)
        {
            v1 = default;
            v2 = default;

            var text = node.Attribute(name);

            if (text == null) return false;

            var parts = text.Split(',');
            if (parts.Length != 2) return false;

            if (!double.TryParse(parts[0].Trim(), out v1)) return false;
            if (!double.TryParse(parts[1].Trim(), out v2)) return false;
            return true;
        }

        public T AsEnum<T>(string name, T defaultValue) where T : struct, IConvertible
        {
            var attr = node.Attribute(name)?.Replace("-", "")?.Replace(" ", "");
            if (attr == null) return defaultValue;

            if (!Enum.TryParse<T>(attr, true, out var value)) return defaultValue;

            return value;
        }
    }
}
