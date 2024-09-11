using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.DTO;

/* Traits
    ? EatsWarrens
    ? FeedsStoat,
    ? LikesHoney,
    ? Wolf,
    ? Bear,
    ? Juvenile,
    ? ProtectsCub,
    ? Undead,
    ? Structure,
    ? Blind,
    Ant => This card contributes to the power of Ant cards
    --- Terrain,
    ? Pelt,
    Uncuttable => This card is immune to the Scissors, Fish Hook and Skinning Knife items
    --- SatisfiesRingTrial,
    Giant => Increases in power by 1 for each turn after you run out of cards TODO Or is this special ability?
    ? Gem,
    ? Fused,
    [Only at campfire] KillsSurvivors => If this card dies during the second campfire upgrade, the upgrade will be without risk for the rest of the run
    [Only at boon lord sacrifice] Goat => If this card is sacrificed, you will get a Boon of the Bone Lord instead of a Minor Boon of the Bone Lord
    --- DeathcardCreationNonOption,
    Lice => If a Pelt is played, this card will be played automatically next to it
 */

/* Special abilities
    [On board] Cat => Transforms into Undead Cat after being sacrified 9 times
    Ant => Power is equal to number of Ants
    RandomCard => Transforms into a random card when drawn
    Lammergeier => Power is equal to half the number of bones
    --- TalkingCardChooser,
    PackMule => Gives 4 cards in hand when this card dies
    Mirror => Power is equal to the power of the card opposite this one
    BellProximity => Power ranges from 4 to 1, depending on which lane you place it in and how close it is to the bell
    --- CagedWolf,
    TrapSpawner => Spawns a Leaping Trap in its place when it dies
    GiantCard => Increases in power by 1 for each turn after you run out of cards
    --- GiantMoon,
    --- GreenMage,
    [On board] JerseyDevil => Transforms into Hungry Child after being sacrified 13 times
    Daus => If a card with the Chime trait is attacked, this card will attack the attacker
    --- BountyHunter
    --- BrokenCoinLeft,
    --- BrokenCoinRight,
    CardsInHand => Power is equal to the number of cards in your hand
    Ouroboros => Health and power increases by 1 permanently each time it dies or is sacrificed
    Shapeshifter => This card disguises itself as another card in the deck and only reveals itself after it is played // TODO Disguise Ijiraq in hand
    SacrificesThisTurn => Power is equal to the number of cards you've sacrificed this turn
    [Only if Lice in Deck] SpawnLice => When played, automatically plays any Pelt Lice cards from your hand or deck in adjacent lanes
    --- GiantShip,
 */

public class CardDTO
{
    public string Name { get; protected set; }
    public int? Health { get; protected set; }
    public string Power { get; protected set; }
    public List<string> Sigils { get; protected set; }
    public bool? IsPlayable { get; protected set; }
    public string Cost { get; protected set; }
    public bool? CanBeSacrificed { get; protected set; }
    public string Tribes { get; protected set; }
    // public string Traits = GetTraits(card);
    // specialAbilities
    public string Note { get; protected set; }

    public CardDTO(CardInfo cardInfo)
    {
        if (cardInfo.name == "!STATIC!GLITCH")
        {
            Name = "Glitched Card";
            return;
        }

        Name = cardInfo.DisplayedNameEnglish;
        Health = cardInfo.Health;
        Power = CalculatePower(cardInfo);
        Sigils = cardInfo.Abilities.Count == 0 ? null : cardInfo.Abilities.Select(a => a.GetRulebookStrings().name).ToList();
        Cost = cardInfo.BonesCost > 0 ? $"{cardInfo.BonesCost} bones" : $"{cardInfo.BloodCost} blood";
        CanBeSacrificed = cardInfo.Sacrificable || cardInfo.HasAbility(Ability.TripleBlood);
        Tribes = cardInfo.tribes.Count == 0 ? null : string.Join(", ", cardInfo.tribes);
    }

    protected CardDTO(CardInfo cardInfo, PlayableCard playableCard) : this(cardInfo)
    {
        Name = $"{cardInfo.DisplayedNameEnglish}{(playableCard.OpponentCard ? null : $" ({(playableCard.InHand ? 12 + PlayerHand.Instance.CardsInHand.IndexOf(playableCard) : BoardManager.Instance.CardsOnBoard.IndexOf(playableCard))})")}";
        Power = CalculatePower(cardInfo, playableCard);
        IsPlayable = !playableCard.InHand ? null : playableCard.CanPlay();

        if (!playableCard.InHand) Cost = null;
        if (playableCard.OpponentCard) CanBeSacrificed = null;

        if (GameState.Ijiraq.Current.SuspiciousCardsInHand.Contains(playableCard)) Note = $"Careful! This card or another {cardInfo.DisplayedNameEnglish} card in your hand is a disguised Ijiraq!";
    }

    public CardDTO(PlayableCard card) : this(card.Info, card) { }

    private static string CalculatePower(CardInfo cardInfo, PlayableCard cardInPlay = null)
    {
        foreach (SpecialTriggeredAbility ability in cardInfo.specialAbilities)
        {
            switch (ability)
            {
                case SpecialTriggeredAbility.Ant:
                    return $"Equal to the number of ants on your side of the board (currently";// TODO {CurrentState.Board.PlayerLanes.Count(c => c != null && c has trait ant)})";

                case SpecialTriggeredAbility.Lammergeier:
                    return $"Equal to half the number of bones you have (currently {GameState.Board.Current.Bones} bones, meaning {GameState.Board.Current.Bones / 2} damage)";

                case SpecialTriggeredAbility.SacrificesThisTurn:
                    return $"Equal to the number of cards you've sacrificed this turn (currently {BoardManager.Instance.SacrificesMadeThisTurn})";

                case SpecialTriggeredAbility.Mirror:
                {
                    string result = "Equal to the power of the card opposite this one";
                    if (cardInPlay && !cardInPlay.InHand) result += $" (currently {cardInPlay.Attack})";
                    return result;
                }

                case SpecialTriggeredAbility.BellProximity:
                {
                    string result = "Ranges from 4 to 1, depending on which lane you place it in. Lane 0 means 4 attack, lane 1 means 3 attack, lane 2 means 2 attack and lane 3 means 1 attack.";
                    if (cardInPlay && !cardInPlay.InHand) result += $" (currently {cardInPlay.Attack})";
                    return result;
                }

                case SpecialTriggeredAbility.CardsInHand:
                    return $"Equal to the number of cards in your hand (currently {GameState.Board.Current.PlayerHand.Count})";
            }
        }

        return cardInfo.Attack.ToString();
    }
}
