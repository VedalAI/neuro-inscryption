using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data;

public sealed class TraitData : IComparable<TraitData>, IEquatable<TraitData>, IEquatable<Trait>, IEquatable<SpecialTriggeredAbility>, IDescribableConvertible
{
    private enum Type
    {
        Trait,
        Ability,
        Custom
    }

    public string Name { get; }
    public string Description { get; }

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

        Describable? describable = TraitStrings.Get(trait);
        if (describable == null) throw new ArgumentException($"Trait {trait} not found in strings");

        Name = describable.Name;
        Description = describable.Description;
    }

    private TraitData(SpecialTriggeredAbility ability)
    {
        _type = Type.Ability;
        _value = ability;

        Describable? describable = TraitStrings.Get(ability);
        if (describable == null) throw new ArgumentException($"Special ability {ability} not found in strings");

        Name = describable.Name;
        Description = describable.Description;
    }

    private TraitData(Describable custom)
    {
        _type = Type.Custom;
        _value = custom.Name;

        Name = custom.Name;
        Description = custom.Description;
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

    public static TraitData Create(Describable custom)
    {
        return new TraitData(custom);
    }

    public static implicit operator TraitData?(Trait trait) => Create(trait);
    public static implicit operator TraitData?(SpecialTriggeredAbility ability) => Create(ability);

    #region Equality

    public int CompareTo(TraitData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(TraitData? other) => _type == other?._type && _value.Equals(other._value);
    public bool Equals(Trait other) => _type == Type.Trait && (Trait) _value == other;
    public bool Equals(SpecialTriggeredAbility other) => _type == Type.Ability && (SpecialTriggeredAbility) _value == other;
    public override bool Equals(object? obj) => obj is TraitData other && Equals(other);
    public override int GetHashCode() => unchecked(((int) _type * 397) ^ _value.GetHashCode());
    public static bool operator ==(TraitData? left, TraitData? right) => Equals(left, right);
    public static bool operator !=(TraitData? left, TraitData? right) => !(left == right);
    public static bool operator ==(TraitData? left, Trait right) => left?.Equals(right) ?? false;
    public static bool operator !=(TraitData? left, Trait right) => !(left == right);
    public static bool operator ==(Trait left, TraitData? right) => right == left;
    public static bool operator !=(Trait left, TraitData? right) => !(left == right);
    public static bool operator ==(TraitData? left, SpecialTriggeredAbility right) => left?.Equals(right) ?? false;
    public static bool operator !=(TraitData? left, SpecialTriggeredAbility right) => !(left == right);
    public static bool operator ==(SpecialTriggeredAbility left, TraitData? right) => right == left;
    public static bool operator !=(SpecialTriggeredAbility left, TraitData? right) => !(left == right);

    #endregion
}
