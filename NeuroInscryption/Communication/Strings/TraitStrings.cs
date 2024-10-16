using System;
using System.Linq;
using System.Reflection;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Strings;

public static class TraitStrings
{
    // Traits

    [Describes(Trait.Ant)]
    private static readonly Description _antContribute = ("Ant", "Counts as an ant for the Ant Power trait.");

    [Describes(Trait.Pelt)]
    private static readonly Description _peltSacrifice = ("Worthless Sacrifice", "Worth nothing if sacrificed to the Bone Lord.");

    [Describes(Trait.Uncuttable)]
    private static readonly Description _uncuttable = ("Uncuttable", "Cannot be cut or hooked using items.");

    [Describes(Trait.Giant)]
    private static readonly Description _giant = ("Final Boss", "Power grows by 1 for each turn after you run out of cards.");

    [Describes(Trait.KillsSurvivors)]
    private static readonly Description _killsSurvivors = ("Poisonous", "If lost at the campfire, the campfire upgrade will be risk-free for the rest of the run.");

    [Describes(Trait.Goat)]
    private static readonly Description _blackGoat = ("Black Goat", "Grants a better boon when sacrificed to the Bone Lord.");

    [Describes(Trait.Lice)]
    private static readonly Description _lice = ("Lice", "When playing a Lice Infested card, automatically plays itself on an adjacent space if possible.");

    // Special Abilities

    [Describes(SpecialTriggeredAbility.Cat)]
    private static readonly Description _cat = ("Cat", "After being sacrificed 9 times, transforms into an Undead Cat (3 power, 6 health).");

    [Describes(SpecialTriggeredAbility.Ant)]
    private static readonly Description _antPower = ("Ant Power", "Power is equal to the number of Ant cards on the same side of the board.");

    [Describes(SpecialTriggeredAbility.RandomCard)]
    private static readonly Description _randomCard = ("Random Card", "When drawn into your hand, transforms into a random card.");

    [Describes(SpecialTriggeredAbility.Lammergeier)]
    private static readonly Description _lammergeier = ("Lammergeier", "Power is equal to half your number of bones.");

    [Describes(SpecialTriggeredAbility.PackMule)]
    private static readonly Description _packMule = ("Pack Mule", "On death, receive 4 random cards in your hand.");

    [Describes(SpecialTriggeredAbility.Mirror)]
    private static readonly Description _mirror = ("Mirror", "Power is equal to the power of the opposing card, if any.");

    [Describes(SpecialTriggeredAbility.BellProximity)]
    private static readonly Description _bellProximity = ("Bell Ringer", "Power ranges from 1 to 4 based on proximity to the bell, which is left of lane 0. Additionally, gains 1 power for every adjacent Chime.");

    [Describes(SpecialTriggeredAbility.TrapSpawner)]
    private static readonly Description _trapSpawner = ("Trap Spawner", "On death, gets replaced by a Leaping Trap (0 power, 1 health, Steel Trap and Mighty Leap sigils).");

    [Describes(SpecialTriggeredAbility.JerseyDevil)]
    private static readonly Description _child13 = ("Child 13", "When sacrificed, transforms back and forth between [0 base power] and [2 base power with Airborne sigil]. After being sacrificed 13 times, it transforms into a Hungry Child (0 power, 1 health).");

    [Describes(SpecialTriggeredAbility.Daus)]
    private static readonly Description _daus = ("Daus", "When a Chime card is attacked, this card will attack the attacker.");

    [Describes(SpecialTriggeredAbility.CardsInHand)]
    private static readonly Description _cardsInHand = ("Card Counter", "Power is equal to the number of cards in your hand.");

    [Describes(SpecialTriggeredAbility.Ouroboros)]
    private static readonly Description _ouroboros = ("Ouroboros", "On death, gains 1 power and 1 health for the rest of the run.");

    [Describes(SpecialTriggeredAbility.Shapeshifter)]
    private static readonly Description _shapeshifter = ("Shapeshifter", "Disguises itself as another card in your deck and only reveals itself after it is played.");

    [Describes(SpecialTriggeredAbility.SacrificesThisTurn)]
    private static readonly Description _sacrificesThisTurn = ("Sacrifice Counter", "Power is equal to the number of cards you've sacrificed this turn.");

    [Describes(SpecialTriggeredAbility.SpawnLice)]
    private static readonly Description _spawnLice = ("Lice Infested", "When played, any Lice cards in your hand or deck are automatically played adjacent to this card if possible.");

    // TODO: Add bait bucket trait (spawn great white)
    // TODO: Add limoncello trait (spawn skeletons)

    // Custom

    public static readonly Description Chime = ("Chime", "When attacked, any Daus cards on the board will attack the attacker.");

    public static readonly Description Hodag = ("Hodag", "Power gained from the Blood Lust sigil is kept for the rest of the run.");

    // ---

    public static Description? Get(Trait trait)
    {
        FieldInfo? fieldInfo = ReflectionCache.GetAllFields(typeof(TraitStrings)).SingleOrDefault(f => f.GetCustomAttribute<DescribesAttribute>()?.Matches(trait) ?? false);
        return fieldInfo != null ? (Description) fieldInfo.GetValue(null) : null;
    }

    public static Description? Get(SpecialTriggeredAbility ability)
    {
        FieldInfo? fieldInfo = ReflectionCache.GetAllFields(typeof(TraitStrings)).SingleOrDefault(f => f.GetCustomAttribute<DescribesAttribute>()?.Matches(ability) ?? false);
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
