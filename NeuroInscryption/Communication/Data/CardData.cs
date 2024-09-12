using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data;

[PublicAPI]
public class CardData
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
    [JsonProperty]
    private string Note => _maybeIjiraq ? $"Careful! This card or another {TargetCardInfo.DisplayedNameEnglish} card in your hand is a disguised Ijiraq!" : null;

    [JsonIgnore]
    public CardInfo TargetCardInfo { get; private set; }
    [JsonIgnore]
    public PlayableCard TargetCard { get; private set; }
    private bool _maybeIjiraq;

    public CardData(CardInfo targetCard)
    {
        TargetCardInfo = targetCard;

        if (targetCard.name == "!STATIC!GLITCH")
        {
            Name = "Glitched Card";
            return;
        }

        Name = targetCard.DisplayedNameEnglish;
        Health = targetCard.Health;
        Power = CalculatePower(targetCard);
        Sigils = targetCard.GetAllRelevantSigilStrings().Select(s => s.Name).ToList().NullIfEmpty();
        Cost = targetCard.BonesCost > 0 ? $"{targetCard.BonesCost} bones" : $"{targetCard.BloodCost} blood";
        CanBeSacrificed = targetCard.Sacrificable || targetCard.HasAbility(Ability.TripleBlood);
        Tribes = targetCard.tribes.Count == 0 ? null : string.Join(", ", targetCard.tribes);
        Traits = targetCard.GetAllRelevantTraitStrings().Select(t => t.Name).ToList().NullIfEmpty();
    }

    protected CardData(CardInfo targetCard, PlayableCard playableCard) : this(targetCard)
    {
        TargetCard = playableCard;

        Name = $"{targetCard.DisplayedNameEnglish}{(playableCard.OpponentCard ? null : $" ({(playableCard.InHand ? 12 + PlayerHand.Instance.CardsInHand.IndexOf(playableCard) : BoardManager.Instance.CardsOnBoard.IndexOf(playableCard))})")}";
        Power = CalculatePower(targetCard, playableCard);
        IsPlayable = !playableCard.InHand ? null : playableCard.CanPlay();

        if (!playableCard.InHand) Cost = null;
        if (playableCard.OpponentCard) CanBeSacrificed = null;

        if (GameState.Ijiraq.Current.SuspiciousCardsInHand.Contains(playableCard)) _maybeIjiraq = true;
    }

    public CardData(PlayableCard card) : this(card.Info, card) { }

    public string GetDescription()
    {
        StringBuilder sb = new($"[{Name}, {Power} power, {Health} health");
        if (Sigils != null) sb.Append($", {string.Join(", ", Sigils)}");
        if (_maybeIjiraq) sb.Append(", maybe Ijiraq");
        sb.Append($", Cost of {Cost}]");
        return sb.ToString();
    }

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
