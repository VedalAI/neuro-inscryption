using NeuroInscryption.Utilities.Strings;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed record DescriptionFormat(FormatString Name, FormatString Info)
{
    public static implicit operator DescriptionFormat((FormatString name, FormatString info) tuple) => new(tuple.name, tuple.info);

    public Description Format(params object[] args)
    {
        return (Name.Format(args), Info.Format(args));
    }
}
