using System;

namespace NeuroInscryption.Communication.Strings;

public readonly record struct Describable(string Name, string Description) : IComparable<Describable>
{
    public static implicit operator Describable((string name, string description) tuple) => new(tuple.name, tuple.description);

    public int CompareTo(Describable other)
    {
        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}
