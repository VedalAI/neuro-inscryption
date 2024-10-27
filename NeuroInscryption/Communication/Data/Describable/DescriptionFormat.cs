using NeuroInscryption.Utilities.Strings;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed record DescriptionFormat(string Name, string Info)
{
    public static implicit operator DescriptionFormat((string name, string info) tuple) => new(tuple.name, tuple.info);

    public Description Format(params object[] args)
    {
        return (StringFormatter.Format(Name, args), StringFormatter.Format(Info, args));
    }
}
