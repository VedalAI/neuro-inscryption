using System.Collections.Generic;
using System.Linq;
using DiskCardGame;

namespace NeuroInscryption.Communication.Strings;

public static class StringsExtensions
{
    private static Describable GetStrings(this Ability ability)
    {
        return ability switch
        {
            Ability.SubmergeSquid => ("Kraken Waterborne", "A card bearing this sigil submerges itself during its opponent's turn. While submerged, opposing creatures attack its owner directly. After emerging, this card will turn into a random Tentacle card."),
            _ => deconstruct(AbilitiesUtil.GetInfo(ability))
        };

        static Describable deconstruct(AbilityInfo info) => (info.rulebookName, info.LocalizedRulebookDescription.Replace("[creature]", Localization.Translate("a card bearing this sigil")));
    }

    private static Describable GetStrings(this Trait trait)
    {
        return trait switch
        {
            Trait.Ant => ("Ant", "A card with this trait contributes to the power of cards with the Ant Power trait."),
            Trait.Pelt => ("Worthless Sacrifice", "If a card with this trait is sacrificed to the Bone Lord, you will not receive any boon."),
            Trait.Uncuttable => ("Uncuttable", "A card with this trait cannot be cut or hooked using items."),
            Trait.Giant => ("Final Boss", "A card with this trait increases its power by 1 for each turn after you run out of cards."),
            Trait.KillsSurvivors => ("Poisonous", "If a card with this trait perishes at the campfire, the campfire upgrade will be without risk for the rest of the run."),
            Trait.Goat => ("Black Goat", "If a card with this trait is sacrificed to the Bone Lord, you will receive a better boon."),
            Trait.Lice => ("Lice", "If a card with the Pelt trait is played, this card will be played automatically next to it if possible."),
            _ => default
        };
    }

    private static Describable GetStrings(this SpecialTriggeredAbility ability)
    {
        return ability switch
        {
            SpecialTriggeredAbility.Cat => ("Cat", "A card bearing this trait transforms into an Undead Cat (3 power, 6 health) after being sacrificed 9 times."),
            SpecialTriggeredAbility.Ant => ("Ant Power", "The power of a card bearing this trait is equal to the number of cards bearing the Ant trait on the same side of the board."),
            SpecialTriggeredAbility.RandomCard => ("Random Card", "A card bearing this trait transforms into a random card when drawn."),
            SpecialTriggeredAbility.Lammergeier => ("Lammergeier", "The power of a card bearing this trait is equal to half the number of bones that you have."),
            SpecialTriggeredAbility.PackMule => ("Pack Mule", "When a card bearing this trait dies, you get 4 random cards in your hand."),
            SpecialTriggeredAbility.Mirror => ("Mirror", "The power of a card bearing this trait is equal to the power of the card opposite it."),
            SpecialTriggeredAbility.BellProximity => ("Leftmost", "The power of a card bearing this trait ranges from 4 to 1, depending on which lane you place it in."),
            SpecialTriggeredAbility.TrapSpawner => ("Trap Spawner", "When a card bearing this trait dies, a Leaping Trap (0 power, 1 health, Steel Trap and Mighty Leap sigils) is played in its place."),
            SpecialTriggeredAbility.JerseyDevil => ("Child 13", "A card bearing this trait transforms back and forth between [0 power] and [2 power with Flying sigil] each time it is sacrificed. After being sacrificed 13 times, it transforms into a Hungry Child (0 power, 1 health)."),
            SpecialTriggeredAbility.Daus => ("Daus", "If a card bearing the Chime trait is attacked, a card bearing this trait will attack the attacker."),
            SpecialTriggeredAbility.CardsInHand => ("Card Counter", "The power of a card bearing this trait is equal to the number of cards in your hand."),
            SpecialTriggeredAbility.Ouroboros => ("Ouroboros", "The power and health of a card bearing this trait permanently increases by 1 each time it dies or is sacrificed."),
            SpecialTriggeredAbility.Shapeshifter => ("Shapeshifter", "A card bearing this trait disguises itself as another card in your deck and only reveals itself after it is played."),
            SpecialTriggeredAbility.SacrificesThisTurn => ("Sacrifice Counter", "The power of a card bearing this trait is equal to the number of cards you've sacrificed this turn."),
            SpecialTriggeredAbility.SpawnLice => ("Lice Infected", "When a card bearing this trait is played, any cards bearing the Lice trait in your hand or deck are played automatically adjacent to it if possible."),
            _ => default,
        };
    }

    private static Describable ChimeTrait => ("Chime", "When a card bearing this trait is attacked, any cards bearing the Daus trait in play will attack the attacker.");

    public static List<Describable> GetAllRelevantSigilStrings(this CardInfo cardInfo)
    {
        List<Describable> result = [];

        result.AddRange(cardInfo.Abilities.Select(a => a.GetStrings()).Where(a => !string.IsNullOrEmpty(a.Name)));
        if (cardInfo.specialAbilities.Contains(SpecialTriggeredAbility.JerseyDevil)) result.Add(Ability.Flying.GetStrings());
        if (cardInfo.specialAbilities.Contains(SpecialTriggeredAbility.TrapSpawner)) result.AddRange([Ability.SteelTrap.GetStrings(), Ability.Reach.GetStrings()]);

        result.Sort();
        return result;
    }

    public static List<Describable> GetAllRelevantTraitStrings(this CardInfo cardInfo)
    {
        List<Describable> result = [];

        result.AddRange(cardInfo.traits.Select(t => t.GetStrings()).Where(t => !string.IsNullOrEmpty(t.Name)));
        result.AddRange(cardInfo.specialAbilities.Select(a => a.GetStrings()).Where(a => !string.IsNullOrEmpty(a.Name)));
        if (cardInfo.name.ToLowerInvariant().Contains("bell")) result.Add(ChimeTrait);

        if (GameFlowManager.Instance.CurrentGameState == DiskCardGame.GameState.CardBattle)
        {
            result.RemoveAll(t => t.Name is "Poisonous" or "Black Goat"); // boon lord stuff we don't care about
        }

        result.Sort();
        return result;
    }
}
