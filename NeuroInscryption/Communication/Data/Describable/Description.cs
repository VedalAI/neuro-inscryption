using System;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed record Description(string Name, string Info) : IComparable<Description>
{
    public static implicit operator Description((string name, string info) tuple) => new(tuple.name, tuple.info);

    public int CompareTo(Description other)
    {
        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}
