using System;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data;

public static class GameState
{
    [PublicAPI]
    public class Board
    {
        public static Board Current => new();

        public readonly int Scale = LifeManager.Instance.Balance;

        public readonly int Bones = ResourcesManager.Instance.PlayerBones;

        public readonly int PlayerLives = RunState.Run.playerLives;

        public readonly Dictionary<string, CardData?> PlayerLanes = BoardManager.Instance.playerSlots.WithIndex()
            .ToDictionary(p => p.index.ToString(), p => !p.item.Card ? null : new CardData(p.item.Card));

        public readonly Dictionary<string, CardData?> OpponentLanes = BoardManager.Instance.opponentSlots.WithIndex()
            .ToDictionary(p => p.index.ToString(), p => !p.item.Card ? null : new CardData(p.item.Card));

        public readonly Dictionary<string, CardData?> OpponentQueueLanes = BoardManager.Instance.opponentSlots
            .Select(BoardManager.Instance.GetCardQueuedForSlot).WithIndex()
            .ToDictionary(p => p.index.ToString(), p => !p.item ? null : new CardData(p.item));

        public readonly List<CardData> PlayerHand = DiskCardGame.PlayerHand.Instance.CardsInHand
            .Select(c => Ijiraq.Current.ExposableIjiraqs.Contains(c) ? new IjiraqCardData(c) : new CardData(c))
            .ToList();

        public readonly List<CardData> RemainingDeck = CardDrawPiles.Instance.Deck.Cards
            .Select(c => Ijiraq.Current.FakeIjiraqsInDeck.Contains(c) ? CardLoader.GetCardByName("Ijiraq") : c)
            .Select(c => new CardData(c))
            .ToList();

        // TODO: Standardize card ids so that empty lanes are taken into account in the index calculation

        [JsonProperty("Note")]
        private readonly string? _note = !Ijiraq.Current.AnythingCouldBeIjiraq ? null : "Be careful, any card in your hand or deck could be a disguised Ijiraq.";

        public readonly Dictionary<string, string>? SigilDescriptions = AllCardsInPlay
            .SelectMany(c => c.GetAllRelevantSigilStrings()).Distinct()
            .ToDictionary(s => s.Name, t => t.Description)
            .NullIfEmpty();

        public readonly Dictionary<string, string>? TraitDescriptions = AllCardsInPlay
            .SelectMany(c => c.GetAllRelevantTraitStrings()).Distinct()
            .ToDictionary(t => t.Name, t => t.Description)
            .NullIfEmpty();

        // ["playerTotem"]
        // ["items"]
        // ["remainingSquirrels"] ?
        // ["enemyTotem"]
        // ["enemyLives"]
        // ["boons"]
        // ["isOpponentSurrendering"] ?
        // ["bossAbilityDescription"] ?

        private static IEnumerable<CardInfo> AllCardsInPlay => BoardManager.Instance.CardsOnBoard.Select(c => c.Info)
            .Concat(DiskCardGame.PlayerHand.Instance.CardsInHand.Select(c => c.Info))
            .Concat(CardDrawPiles.Instance.Deck.Cards);
    }

    public class Ijiraq
    {
        /// <summary>
        /// All cards in the player's deck that should be faked as Ijiraqs.
        /// </summary>
        public readonly List<CardInfo> FakeIjiraqsInDeck;

        /// <summary>
        /// All Ijiraqs in the player's hand that should be exposed as Ijiraqs.
        /// </summary>
        public readonly List<PlayableCard> ExposableIjiraqs;

        /// <summary>
        /// All cards in the player's hand that could be Ijiraqs.
        /// </summary>
        public readonly List<PlayableCard> SuspiciousCardsInHand;

        /// <summary>
        /// All Ijiraqs in the deck.
        /// </summary>
        private readonly List<CardInfo> _ijiraqsInDeck;

        public bool AnythingCouldBeIjiraq => _ijiraqsInDeck.Count > 0 || FakeIjiraqsInDeck.Count > 0;

        public static Ijiraq Current => new();

        private Ijiraq()
        {
            List<PlayableCard> ijiraqsInHand = PlayerHand.Instance.CardsInHand
                .Where(card => card.gameObject.GetComponent<Shapeshifter>())
                .Where(card => !string.Equals(card.Info.DisplayedNameEnglish, "Ijiraq", StringComparison.OrdinalIgnoreCase))
                .ToList();

            List<PlayableCard> tempIjiraqsToFake = [..ijiraqsInHand];
            FakeIjiraqsInDeck = [];
            foreach (CardInfo cardInDeck in CardDrawPiles.Instance.Deck.Cards)
            {
                foreach (PlayableCard ijiraq in tempIjiraqsToFake)
                {
                    // If this card in the deck is the same as the Ijiraq disguise, then we fake this card as being an Ijiraq
                    if (cardInDeck.IsIdentical(ijiraq.Info))
                    {
                        FakeIjiraqsInDeck.Add(cardInDeck);
                        tempIjiraqsToFake.Remove(ijiraq);
                        break;
                    }
                }
            }

            ExposableIjiraqs = ijiraqsInHand
                .Where(ijr => !CardDrawPiles.Instance.Deck.Cards.Any(card => card.IsIdentical(ijr.Info)))
                .Where(ijr => !PlayerHand.Instance.cardsInHand.Except([ijr]).Any(card => card.Info.IsIdentical(ijr.Info)))
                .ToList();

            SuspiciousCardsInHand = PlayerHand.Instance.CardsInHand
                .Where(card => !CardDrawPiles.Instance.Deck.Cards.Any(deckCard => deckCard.IsIdentical(card.Info)))
                .Where(card => ijiraqsInHand.Any(ijr => ijr.Info.IsIdentical(card.Info)))
                .ToList();

            _ijiraqsInDeck = CardDrawPiles.Instance.Deck.Cards
                .Where(cardInfo => cardInfo.SpecialAbilities.Contains(SpecialTriggeredAbility.Shapeshifter))
                .ToList();
        }
    }
}
