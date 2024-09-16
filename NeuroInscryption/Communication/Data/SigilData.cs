using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data;

public sealed class SigilData : IComparable<SigilData>, IEquatable<SigilData>, IEquatable<Ability>
{
    public readonly string Name;
    public readonly string Description;
    public readonly Ability Sigil;

    private SigilData(Ability ability)
    {
        Sigil = ability;

        if (ability == Ability.SubmergeSquid)
        {
            Name = SigilStrings.SubmergeSquidName;
            Description = SigilStrings.SubmergeSquidDescription;
        }
        else
        {
            AbilityInfo info = AbilitiesUtil.GetInfo(ability);
            Name = info.rulebookName;
            Description = info.LocalizedRulebookDescription.Replace("[creature]", Localization.Translate("a card bearing this sigil"));
            Description = char.ToUpper(Description[0]) + Description[1..];
            // TODO: Are card definitions included here? ("A rabbit is defined as 0 power, 1 health")
        }
    }

    public static SigilData Create(Ability sigil) => new(sigil);

    public static implicit operator SigilData(Ability sigil) => Create(sigil);

    #region Equality

    public int CompareTo(SigilData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(SigilData? other) => Sigil == other?.Sigil;
    public bool Equals(Ability other) => Sigil == other;
    public override bool Equals(object? obj) => obj is SigilData other && Equals(other);
    public override int GetHashCode() => (int) Sigil;
    public static bool operator ==(SigilData? left, SigilData? right) => Equals(left, right);
    public static bool operator !=(SigilData? left, SigilData? right) => !(left == right);
    public static bool operator ==(SigilData? left, Ability right) => left?.Equals(right) ?? false;
    public static bool operator !=(SigilData? left, Ability right) => !(left == right);
    public static bool operator ==(Ability left, SigilData? right) => right == left;
    public static bool operator !=(Ability left, SigilData? right) => !(left == right);

    #endregion
}
