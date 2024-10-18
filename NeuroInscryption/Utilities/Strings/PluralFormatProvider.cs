using System;

namespace NeuroInscryption.Utilities.Strings;

internal sealed class PluralFormatProvider : IFormatProvider, ICustomFormatter
{
    public object GetFormat(Type formatType) => this;

    public string Format(string? format, object? arg, IFormatProvider formatProvider)
    {
        if (format == null || arg == null)
        {
            return arg?.ToString() ?? "null";
        }

        string[] formats = format.Split('|');
        if (arg is int count)
        {
            return count == 1 ? formats[0] : formats[1];
        }

        return arg.ToString();
    }
}
