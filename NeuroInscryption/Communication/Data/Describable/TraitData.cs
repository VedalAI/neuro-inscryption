using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed class TraitData : IComparable<TraitData>, IEquatable<TraitData>, IDescribable
{
    private enum Type
    {
        Trait,
        Ability,
        Custom
    }

    public string Name { get; }
    public string Info { get; }

    private readonly Type _type;
    private readonly object _value;

    public bool TryGetTrait(out Trait trait)
    {
        if (_type == Type.Trait)
        {
            trait = (Trait) _value;
            return true;
        }

        trait = default;
        return false;
    }

    public bool TryGetAbility(out SpecialTriggeredAbility ability)
    {
        if (_type == Type.Ability)
        {
            ability = (SpecialTriggeredAbility) _value;
            return true;
        }

        ability = default;
        return false;
    }

    private TraitData(Trait trait, Description description)
    {
        _type = Type.Trait;
        _value = trait;

        Name = description.Name;
        Info = description.Info;
    }

    private TraitData(SpecialTriggeredAbility ability, Description description)
    {
        _type = Type.Ability;
        _value = ability;

        Name = description.Name;
        Info = description.Info;
    }

    private TraitData(Description custom)
    {
        _type = Type.Custom;
        _value = custom.Name;

        Name = custom.Name;
        Info = custom.Info;
    }

    public static TraitData? Create(Trait trait)
    {
        Description? description = TraitStrings.Get(trait);
        if (description == null) return null;

        return new TraitData(trait, description);
    }

    public static TraitData? Create(SpecialTriggeredAbility ability)
    {
        Description? description = TraitStrings.Get(ability);
        if (description == null) return null;

        return new TraitData(ability, description);
    }

    public static TraitData Create(Description custom)
    {
        return new TraitData(custom);
    }

    #region Equality

    public int CompareTo(TraitData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(TraitData? other) => _type == other?._type && _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is TraitData other && Equals(other);
    public override int GetHashCode() => unchecked(((int) _type * 397) ^ _value.GetHashCode());
    public static bool operator ==(TraitData? left, TraitData? right) => Equals(left, right);
    public static bool operator !=(TraitData? left, TraitData? right) => !(left == right);

    #endregion
}
