using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed class SigilData : IComparable<SigilData>, IEquatable<SigilData>, IDescribable
{
    public string Name { get; }
    public string Info { get; }
    public Ability Sigil { get; }

    private SigilData(Ability sigil)
    {
        Sigil = sigil;
        Description desc = SigilStrings.Get(sigil);
        Name = desc.Name;
        Info = desc.Info;
    }

    public static SigilData Create(Ability sigil) => new(sigil);

    #region Equality

    public int CompareTo(SigilData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(SigilData? other) => Sigil == other?.Sigil;
    public override bool Equals(object? obj) => obj is SigilData other && Equals(other);
    public override int GetHashCode() => (int) Sigil;
    public static bool operator ==(SigilData? left, SigilData? right) => Equals(left, right);
    public static bool operator !=(SigilData? left, SigilData? right) => !(left == right);

    #endregion
}
