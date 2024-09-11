using System;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.DTO;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication;

public static class GameState
{
    public class Board
    {
        public static Board Current => new();

        public int Scale => LifeManager.Instance.Balance;

        public int Bones => ResourcesManager.Instance.PlayerBones;

        public int PlayerLives => RunState.Run.playerLives;

        public Dictionary<string, CardDTO> PlayerLanes => BoardManager.Instance.playerSlots
            .Select((slot, index) => (slot, index))
            .ToDictionary(p => p.index.ToString(), p => !p.slot.Card ? null : new CardDTO(p.slot.Card));

        public Dictionary<string, CardDTO> OpponentLanes => BoardManager.Instance.opponentSlots
            .Select((slot, index) => (slot, index))
            .ToDictionary(p => p.index.ToString(), p => !p.slot.Card ? null : new CardDTO(p.slot.Card));

        public Dictionary<string, CardDTO> OpponentQueueLanes => BoardManager.Instance.opponentSlots
            .Select(BoardManager.Instance.GetCardQueuedForSlot)
            .Select((card, index) => (card, index))
            .ToDictionary(p => p.index.ToString(), p => !p.card ? null : new CardDTO(p.card));

        public List<CardDTO> PlayerHand => DiskCardGame.PlayerHand.Instance.CardsInHand
            .Select(c => Ijiraq.Current.ExposableIjiraqs.Contains(c) ? new CardDTO(CardLoader.GetCardByName("Ijiraq"), c) : new CardDTO(c))
            .ToList();

        public List<CardDTO> RemainingDeck => CardDrawPiles.Instance.Deck.Cards
            .Select(c => Ijiraq.Current.FakeIjiraqsInDeck.Contains(c) ? CardLoader.GetCardByName("Ijiraq") : c)
            .Select(c => new CardDTO(c))
            .ToList();

        public string Note => !Ijiraq.Current.AnythingCouldBeIjiraq ? null : "Be careful, any card in your hand or deck could be a disguised Ijiraq.";

        // ["playerTotem"]
        // ["items"]
        // ["remainingDeck"]
        // ["remainingSquirrels"] ?
        // ["enemyTotem"]
        // ["enemyLives"]
        // ["boons"]
        // ["isOpponentSurrendering"] ?
        // ["bossAbilityDescription"] ?
    }

    public class Ijiraq
    {
        /// <summary>
        /// All Ijiraqs in the player's hand that are disguised as other cards.<br/>
        /// Ijiraqs disguised as Ijiraqs are ignored.
        /// </summary>
        public readonly List<PlayableCard> IjiraqsInHand;

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
        public readonly List<PlayableCard> PotentialIjiraqsInHand;

        /// <summary>
        /// All Ijiraqs in the deck.
        /// </summary>
        public readonly List<CardInfo> IjiraqsInDeck;

        public bool AnythingCouldBeIjiraq => IjiraqsInDeck.Count > 0 || FakeIjiraqsInDeck.Count > 0;

        public static Ijiraq Current => new();

        private Ijiraq()
        {
            IjiraqsInHand = PlayerHand.Instance.CardsInHand
                .Where(card => card.gameObject.GetComponent<Shapeshifter>())
                .Where(card => !string.Equals(card.Info.DisplayedNameEnglish, "Ijiraq", StringComparison.OrdinalIgnoreCase))
                .ToList();

            List<PlayableCard> tempIjiraqsToFake = [..IjiraqsInHand];
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

            ExposableIjiraqs = IjiraqsInHand
                .Where(ijr => !CardDrawPiles.Instance.Deck.Cards.Any(card => card.IsIdentical(ijr.Info)))
                .Where(ijr => !PlayerHand.Instance.cardsInHand.Any(card => card.Info.IsIdentical(ijr.Info)))
                .ToList();

            PotentialIjiraqsInHand = PlayerHand.Instance.CardsInHand
                .Where(card => IjiraqsInHand.Any(ijr => ijr.Info.IsIdentical(card.Info)))
                .ToList();

            IjiraqsInDeck = CardDrawPiles.Instance.Deck.Cards
                .Where(cardInfo => cardInfo.specialAbilities.Contains(SpecialTriggeredAbility.Shapeshifter))
                .ToList();
        }
    }
}
