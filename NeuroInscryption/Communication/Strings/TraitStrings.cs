using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;

namespace NeuroInscryption.Communication.Strings;

public static class TraitStrings
{
    private static readonly Dictionary<Trait, Description> _traitDescriptions = new()
    {
        [Trait.Ant] = ("Ant", "Counts as an ant for the Ant Power trait."),
        [Trait.Pelt] = ("Worthless Sacrifice", "Worth nothing if sacrificed to the Bone Lord."),
        [Trait.Uncuttable] = ("Uncuttable", "Cannot be cut or hooked using items."),
        [Trait.Giant] = ("Final Boss", "Power grows by 1 for each turn after you run out of cards."),
        [Trait.KillsSurvivors] = ("Poisonous", "If lost at the campfire, the campfire upgrade will be risk-free for the rest of the run."),
        [Trait.Goat] = ("Black Goat", "Grants a better boon when sacrificed to the Bone Lord."),
        [Trait.Lice] = ("Lice", "When playing a Lice Infested card, automatically plays itself on an adjacent space if possible."),
    };

    private static readonly Dictionary<SpecialTriggeredAbility, Description> _abilityDescriptions = new()
    {
        [SpecialTriggeredAbility.Cat] = ("Cat", "After being sacrificed 9 times, transforms into an Undead Cat (3 power, 6 health)."),
        [SpecialTriggeredAbility.Ant] = ("Ant Power", "Power is equal to the number of Ant cards on the same side of the board."),
        [SpecialTriggeredAbility.RandomCard] = ("Random Card", "When drawn into your hand, transforms into a random card."),
        [SpecialTriggeredAbility.Lammergeier] = ("Lammergeier", "Power is equal to half your number of bones."),
        [SpecialTriggeredAbility.PackMule] = ("Pack Mule", "On death, receive 4 random cards in your hand."),
        [SpecialTriggeredAbility.Mirror] = ("Mirror", "Power is equal to the power of the opposing card, if any. Otherwise 0."),
        [SpecialTriggeredAbility.BellProximity] = ("Bell Ringer", "Power ranges from 1 to 4 based on proximity to the bell, which is left of lane 0. Additionally, gains 1 power for every adjacent Chime."), // todo: count lanes from 1
        [SpecialTriggeredAbility.TrapSpawner] = ("Trap Spawner", "On death, gets replaced by a Leaping Trap (0 power, 1 health, Steel Trap and Mighty Leap)."),
        [SpecialTriggeredAbility.JerseyDevil] = ("Child 13", "When sacrificed, transforms back and forth between [0 base power] and [2 base power, Airborne]. After being sacrificed 13 times, it transforms into a Hungry Child (0 power, 1 health)."),
        [SpecialTriggeredAbility.Daus] = ("Daus", "When a Chime card is attacked, this card will attack the attacker."),
        [SpecialTriggeredAbility.CardsInHand] = ("Card Counter", "Power is equal to the number of cards in your hand."),
        [SpecialTriggeredAbility.Ouroboros] = ("Ouroboros", "On death, gains 1 power and 1 health for the rest of the run."),
        [SpecialTriggeredAbility.Shapeshifter] = ("Shapeshifter", "Disguises itself as another card in your deck and only reveals itself after it is played."),
        [SpecialTriggeredAbility.SacrificesThisTurn] = ("Sacrifice Counter", "Power is equal to the number of cards you've sacrificed this turn."),
        [SpecialTriggeredAbility.SpawnLice] = ("Lice Infested", "When played, any Lice cards in your hand or deck are automatically played in an open space if possible.")
    };

    public static readonly Description Chime = ("Chime", "When attacked, any Daus cards on the board will attack the attacker.");
    public static readonly Description Hodag = ("Hodag", "Power gained from the Blood Lust sigil is kept for the rest of the run.");
    public static readonly Description BaitBucket = ("Bait Bucket", "On death, gets replaced by a Great White (4 power, 2 health, Waterborne).");
    public static readonly Description Limoncello = ("Mutiny", "After its turn, spawns some Skeleton Crew [2 power, 1 health, Brittle] on your side depending on how low its health is.");

    public static Description? Get(Trait trait) => _traitDescriptions.TryGetValue(trait, out Description description) ? description : null;
    public static Description? Get(SpecialTriggeredAbility ability) => _abilityDescriptions.TryGetValue(ability, out Description description) ? description : null;
}
