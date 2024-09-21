using System;
using System.Linq;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.Describable;

namespace NeuroInscryption.Communication.Strings;

public static class TraitStrings
{
    // Traits

    [Describes(Trait.Ant)]
    private static readonly Description _antContribute = ("Ant", "A card with this trait contributes to the power of cards with the Ant Power trait.");

    [Describes(Trait.Pelt)]
    private static readonly Description _peltSacrifice = ("Worthless Sacrifice", "If a card with this trait is sacrificed to the Bone Lord, you will not receive any boon.");

    [Describes(Trait.Uncuttable)]
    private static readonly Description _uncuttable = ("Uncuttable", "A card with this trait cannot be cut or hooked using items.");

    [Describes(Trait.Giant)]
    private static readonly Description _giant = ("Final Boss", "A card with this trait increases its power by 1 for each turn after you run out of cards.");

    [Describes(Trait.KillsSurvivors)]
    private static readonly Description _killsSurvivors = ("Poisonous", "If a card with this trait perishes at the campfire, the campfire upgrade will be without risk for the rest of the run.");

    [Describes(Trait.Goat)]
    private static readonly Description _blackGoat = ("Black Goat", "If a card with this trait is sacrificed to the Bone Lord, you will receive a better boon.");

    [Describes(Trait.Lice)]
    private static readonly Description _lice = ("Lice", "If a card with the Lice Infected trait is played, this card will be played automatically next to it if possible.");

    // Special Abilities

    [Describes(SpecialTriggeredAbility.Cat)]
    private static readonly Description _cat = ("Cat", "A card bearing this trait transforms into an Undead Cat (3 power, 6 health) after being sacrificed 9 times.");

    [Describes(SpecialTriggeredAbility.Ant)]
    private static readonly Description _antPower = ("Ant Power", "The power of a card bearing this trait is equal to the number of cards bearing the Ant trait on the same side of the board.");

    [Describes(SpecialTriggeredAbility.RandomCard)]
    private static readonly Description _randomCard = ("Random Card", "A card bearing this trait transforms into a random card when drawn.");

    [Describes(SpecialTriggeredAbility.Lammergeier)]
    private static readonly Description _lammergeier = ("Lammergeier", "The power of a card bearing this trait is equal to half the number of bones that you have.");

    [Describes(SpecialTriggeredAbility.PackMule)]
    private static readonly Description _packMule = ("Pack Mule", "When a card bearing this trait dies, you get 4 random cards in your hand.");

    [Describes(SpecialTriggeredAbility.Mirror)]
    private static readonly Description _mirror = ("Mirror", "The power of a card bearing this trait is equal to the power of the card opposite it.");

    [Describes(SpecialTriggeredAbility.BellProximity)]
    private static readonly Description _bellProximity = ("Bell Ringer", "The base power of a card bearing this trait ranges from 4 to 1, depending on how close it is to the bell. The bell is located to the left of lane 0. Additionally, this card gains 1 additional power for every Chime it is next to.");

    [Describes(SpecialTriggeredAbility.TrapSpawner)]
    private static readonly Description _trapSpawner = ("Trap Spawner", "When a card bearing this trait dies, a Leaping Trap (0 power, 1 health, Steel Trap and Mighty Leap sigils) is played in its place.");

    [Describes(SpecialTriggeredAbility.JerseyDevil)]
    private static readonly Description _child13 = ("Child 13", "A card bearing this trait transforms back and forth between [0 base power] and [2 base power with Airborne sigil] each time it is sacrificed. After being sacrificed 13 times, it transforms into a Hungry Child (0 power, 1 health).");

    [Describes(SpecialTriggeredAbility.Daus)]
    private static readonly Description _daus = ("Daus", "If a card bearing the Chime trait is attacked, a card bearing this trait will attack the attacker.");

    [Describes(SpecialTriggeredAbility.CardsInHand)]
    private static readonly Description _cardsInHand = ("Card Counter", "The power of a card bearing this trait is equal to the number of cards in your hand.");

    [Describes(SpecialTriggeredAbility.Ouroboros)]
    private static readonly Description _ouroboros = ("Ouroboros", "The power and health of a card bearing this trait permanently increases by 1 each time it dies or is sacrificed.");

    [Describes(SpecialTriggeredAbility.Shapeshifter)]
    private static readonly Description _shapeshifter = ("Shapeshifter", "A card bearing this trait disguises itself as another card in your deck and only reveals itself after it is played.");

    [Describes(SpecialTriggeredAbility.SacrificesThisTurn)]
    private static readonly Description _sacrificesThisTurn = ("Sacrifice Counter", "The power of a card bearing this trait is equal to the number of cards you've sacrificed this turn.");

    [Describes(SpecialTriggeredAbility.SpawnLice)]
    private static readonly Description _spawnLice = ("Lice Infested", "When a card bearing this trait is played, any cards bearing the Lice trait in your hand or deck are played automatically adjacent to it if possible.");

    // Custom

    public static readonly Description Chime = ("Chime", "When a card bearing this trait is attacked, any cards bearing the Daus trait in play will attack the attacker.");

    // ---

    public static Description? Get(Trait trait)
    {
        FieldInfo? fieldInfo = AccessTools.GetDeclaredFields(typeof(TraitStrings)).SingleOrDefault(f => f.GetCustomAttribute<DescribesAttribute>()?.Matches(trait) ?? false);
        return fieldInfo != null ? (Description) fieldInfo.GetValue(null) : null;
    }

    public static Description? Get(SpecialTriggeredAbility ability)
    {
        FieldInfo? fieldInfo = AccessTools.GetDeclaredFields(typeof(TraitStrings)).SingleOrDefault(f => f.GetCustomAttribute<DescribesAttribute>()?.Matches(ability) ?? false);
        return fieldInfo != null ? (Description) fieldInfo.GetValue(null) : default;
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field)]
file class DescribesAttribute : Attribute
{
    private readonly Type _type;
    private readonly Enum _value;

    public DescribesAttribute(Trait trait)
    {
        _type = typeof(Trait);
        _value = trait;
    }

    public DescribesAttribute(SpecialTriggeredAbility ability)
    {
        _type = typeof(SpecialTriggeredAbility);
        _value = ability;
    }

    public bool Matches(Trait trait) => _type == typeof(Trait) && _value.Equals(trait);
    public bool Matches(SpecialTriggeredAbility ability) => _type == typeof(SpecialTriggeredAbility) && _value.Equals(ability);
}
