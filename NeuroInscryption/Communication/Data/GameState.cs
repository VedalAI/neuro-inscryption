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
        public static Board Calculate() => new();

        public readonly int Scale;
        public readonly int Bones;
        public readonly int PlayerLives;

        public readonly Dictionary<string, CardData?> PlayerLanes;
        public readonly Dictionary<string, CardData?> OpponentLanes;
        public readonly Dictionary<string, CardData?> OpponentQueueLanes;

        public readonly List<CardData> PlayerHand;
        public readonly List<CardData> RemainingDeck;

        public readonly Dictionary<string, string>? SigilDescriptions;
        public readonly Dictionary<string, string>? TraitDescriptions;

        [JsonProperty("Note")]
        private string? Note => Ijiraq.AnythingCouldBeIjiraq ? StateStrings.IjiraqPresent : null;

        // Excluded from JSON

        [JsonIgnore]
        public readonly IjiraqState Ijiraq;

        private Board()
        {
            Ijiraq = new IjiraqState();

            Scale = LifeManager.Instance.Balance;
            Bones = ResourcesManager.Instance.PlayerBones;
            PlayerLives = RunState.Run.playerLives;

            PlayerLanes = BoardManager.Instance.playerSlots.ToIndexedDictionary(slot => CardData.Create(slot.Card, this))!;
            OpponentLanes = BoardManager.Instance.opponentSlots.ToIndexedDictionary(slot => CardData.Create(slot.Card, this))!;
            OpponentQueueLanes = BoardManager.Instance.opponentSlots.Select(BoardManager.Instance.GetCardQueuedForSlot).ToIndexedDictionary(card => CardData.Create(card, this))!;

            PlayerHand = DiskCardGame.PlayerHand.Instance.CardsInHand.Select(c => CardData.Create(c, this)).ToList();
            RemainingDeck = CardDrawPiles.Instance.Deck.Cards.Select((c, i) => CardData.CreateFromDeck(c, i, this)).ToList();

            List<CardData> allCardsInPlay = [..PlayerLanes.Values, ..OpponentLanes.Values, ..OpponentQueueLanes.Values, ..PlayerHand, ..RemainingDeck];
            allCardsInPlay.RemoveAll(data => data == null);

            SigilDescriptions = allCardsInPlay
                .SelectMany(c => c.RelevantSigils).Distinct()
                .ToDictionary(s => s.Name, t => t.Description)
                .NullIfEmpty();
            TraitDescriptions = allCardsInPlay
                .SelectMany(c => c.Traits).Distinct()
                .ToDictionary(t => t.Name, t => t.Description)
                .NullIfEmpty();
        }

        // ["playerTotem"]
        // ["items"]
        // ["remainingSquirrels"] ?
        // ["enemyTotem"]
        // ["enemyLives"]
        // ["boons"]
        // ["isOpponentSurrendering"] ?
        // ["bossAbilityDescription"] ?

        public class IjiraqState
        {
            /// <summary>
            /// All cards in the player's deck that should be faked as Ijiraqs.
            /// </summary>
            public readonly List<int> FakeIjiraqsInDeck;

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

            public IjiraqState()
            {
                // TODO: last card is never an Ijiraq

                List<PlayableCard> ijiraqsInHand = DiskCardGame.PlayerHand.Instance.CardsInHand
                    .Where(card => card.gameObject.GetComponent<Shapeshifter>())
                    .Where(card => !string.Equals(card.Info.DisplayedNameEnglish, "Ijiraq", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                List<PlayableCard> tempIjiraqsToFake = [..ijiraqsInHand];
                FakeIjiraqsInDeck = [];
                for (int deckIndex = 0; deckIndex < CardDrawPiles.Instance.Deck.Cards.Count; deckIndex++)
                {
                    CardInfo cardInDeck = CardDrawPiles.Instance.Deck.Cards[deckIndex];

                    foreach (PlayableCard ijiraq in tempIjiraqsToFake)
                    {
                        // If this card in the deck is the same as the Ijiraq disguise, then we fake this card as being an Ijiraq
                        if (cardInDeck.IsIdentical(ijiraq.Info))
                        {
                            FakeIjiraqsInDeck.Add(deckIndex);
                            tempIjiraqsToFake.Remove(ijiraq);
                            break;
                        }
                    }
                }

                ExposableIjiraqs = ijiraqsInHand
                    .Where(ijr => !CardDrawPiles.Instance.Deck.Cards.Any(card => card.IsIdentical(ijr.Info)))
                    .Where(ijr => !DiskCardGame.PlayerHand.Instance.cardsInHand.Except([ijr]).Any(card => card.Info.IsIdentical(ijr.Info)))
                    .ToList();

                SuspiciousCardsInHand = DiskCardGame.PlayerHand.Instance.CardsInHand
                    .Where(card => !CardDrawPiles.Instance.Deck.Cards.Any(deckCard => deckCard.IsIdentical(card.Info)))
                    .Where(card => ijiraqsInHand.Any(ijr => ijr.Info.IsIdentical(card.Info)))
                    .ToList();

                _ijiraqsInDeck = CardDrawPiles.Instance.Deck.Cards
                    .Where(cardInfo => cardInfo.SpecialAbilities.Contains(SpecialTriggeredAbility.Shapeshifter))
                    .ToList();
            }
        }
    }
}
