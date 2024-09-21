namespace NeuroInscryption.Communication.Data;

public sealed record DescribableFormat(string Name, string Description)
{
    public static implicit operator DescribableFormat((string name, string description) tuple) => new(tuple.name, tuple.description);

    public Describable Format(params object[] args)
    {
        return (string.Format(Name, args), string.Format(Description, args));
    }
}
