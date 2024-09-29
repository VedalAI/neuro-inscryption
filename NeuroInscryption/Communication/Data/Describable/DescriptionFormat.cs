using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed record DescriptionFormat(string Name, string Info)
{
    public static implicit operator DescriptionFormat((string name, string info) tuple) => new(tuple.name, tuple.info);

    public Description Format(params object[] args)
    {
        return (string.Format(Name, args), string.Format(Info, args));
    }

    public Description FormatPlural(params object[] args)
    {
        return (PluralFormatProvider.Format(Name, args), PluralFormatProvider.Format(Info, args));
    }
}
