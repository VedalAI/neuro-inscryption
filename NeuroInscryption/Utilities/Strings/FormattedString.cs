namespace NeuroInscryption.Utilities.Strings;

public sealed class FormatString(string str)
{
    private readonly string _str = str;

    public string Format(params object?[] args)
    {
        return string.Format(new PluralFormatProvider(), _str, args);
    }

    public static implicit operator FormatString(string str)
    {
        return new FormatString(str);
    }

    public static FormatString operator+(FormatString a, FormatString b)
    {
        return new FormatString(a._str + b._str);
    }
}
