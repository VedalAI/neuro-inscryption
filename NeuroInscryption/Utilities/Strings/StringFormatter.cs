namespace NeuroInscryption.Utilities.Strings;

public static class StringFormatter
{
    public static string Format(string format, params object?[] args)
    {
        return string.Format(new PluralFormatProvider(), format, args);
    }
}