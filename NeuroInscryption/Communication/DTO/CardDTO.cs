using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.DTO;

[PublicAPI]
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
    public List<string> Traits { get; protected set; }
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
        Sigils = cardInfo.Abilities.Count == 0 ? null : cardInfo.GetAllRelevantSigilStrings().Select(s => s.Name).ToList();
        Cost = cardInfo.BonesCost > 0 ? $"{cardInfo.BonesCost} bones" : $"{cardInfo.BloodCost} blood";
        CanBeSacrificed = cardInfo.Sacrificable || cardInfo.HasAbility(Ability.TripleBlood);
        Tribes = cardInfo.tribes.Count == 0 ? null : string.Join(", ", cardInfo.tribes);
        Traits = cardInfo.GetAllRelevantTraitStrings().Select(t => t.Name).ToList();
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
