using System;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using UnityEngine;

namespace NeuroInscryption.Communication.DTO;

public sealed class CardDTO(PlayableCard card)
{
    public readonly string Name = $"{card.Info.DisplayedNameEnglish} {(card.OpponentCard ? null : $"({(card.InHand ? PlayerHand.Instance.CardsInHand.IndexOf(card) : BoardManager.Instance.CardsOnBoard.IndexOf(card))})")}";
    public readonly int Health = card.Info.Health;
    public readonly string Power = CalculatePower(card);
    // sigils
    public readonly bool? IsPlayable = !card.InHand ? null : card.CanPlay();
    public readonly string Cost = !card.InHand ? null : card.Info.BonesCost > 0 ? $"{card.Info.BonesCost} bones" : $"{card.Info.BloodCost} blood";
    public readonly bool? CanBeSacrificed = card.OpponentCard ? null : card.Info.Sacrificable || card.HasAbility(Ability.TripleBlood);
    public readonly string Tribes = card.Info.tribes.Count == 0 ? null : string.Join(", ", card.Info.tribes);
    // public readonly string Traits = GetTraits(card);

    private static string CalculatePower(PlayableCard card)
    {
        foreach ((SpecialTriggeredAbility ability, _) in card.TriggerHandler.specialAbilities)
        {
            switch (ability)
            {
                case SpecialTriggeredAbility.Ant:
                    return $"Equal to the number of ants on your side of the board (currently";// TODO {CurrentState.Board.PlayerLanes.Count(c => c != null && c has trait ant)})";

                case SpecialTriggeredAbility.Lammergeier:
                    return $"Equal to half the number of bones you have (currently {CurrentState.Board.Bones} bones, meaning {CurrentState.Board.Bones / 2} damage)";

                case SpecialTriggeredAbility.SacrificesThisTurn:
                    return $"Equal to the number of cards you've sacrificed this turn (currently {BoardManager.Instance.SacrificesMadeThisTurn})";

                case SpecialTriggeredAbility.Mirror:
                {
                    string result = "Equal to the power of the card opposite this one";
                    if (!card.InHand) result += $" (currently {card.Attack})";
                    return result;
                }

                case SpecialTriggeredAbility.BellProximity:
                {
                    string result = "Ranges from 4 to 1, depending on which lane you place it in. Lane 0 means 4 attack, lane 1 means 3 attack, lane 2 means 2 attack and lane 3 means 1 attack.";
                    if (card.InHand) result += $" (currently {card.Attack})";
                    return result;
                }

                case SpecialTriggeredAbility.CardsInHand:
                    return $"Equal to the number of cards in your hand (currently {CurrentState.Board.PlayerHand.Count})";
            }
        }

        return card.Attack.ToString();
    }

    public static CardDTO Create(PlayableCard card) => card ? new CardDTO(card) : null;

    public static CardDTO Create(CardSlot slot) => Create(slot.Card);
}
