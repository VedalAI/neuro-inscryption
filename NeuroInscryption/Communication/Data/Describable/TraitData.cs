using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed class TraitData : IComparable<TraitData>, IEquatable<TraitData>, IDescriptionConvertible
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

    private TraitData(Trait trait)
    {
        _type = Type.Trait;
        _value = trait;

        Description? describable = TraitStrings.Get(trait);
        if (describable == null) throw new ArgumentException($"Trait {trait} not found in strings");

        Name = describable.Name;
        Info = describable.Info;
    }

    private TraitData(SpecialTriggeredAbility ability)
    {
        _type = Type.Ability;
        _value = ability;

        Description? describable = TraitStrings.Get(ability);
        if (describable == null) throw new ArgumentException($"Special ability {ability} not found in strings");

        Name = describable.Name;
        Info = describable.Info;
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
        try
        {
            return new TraitData(trait);
        }
        catch
        {
            return null;
        }
    }

    public static TraitData? Create(SpecialTriggeredAbility ability)
    {
        try
        {
            return new TraitData(ability);
        }
        catch
        {
            return null;
        }
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
