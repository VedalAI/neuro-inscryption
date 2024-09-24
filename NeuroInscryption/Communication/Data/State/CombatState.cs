using System;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using ItemData = NeuroInscryption.Communication.Data.Serializable.ItemData;

namespace NeuroInscryption.Communication.Data.State;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class CombatState : IGameState
{
    public static CombatState Calculate() => new();

    public readonly int Scale;
    public readonly int Bones;
    public readonly int PlayerLives;

    public readonly IReadOnlyDictionary<string, PlayableCardData?> PlayerLanes;
    public readonly IReadOnlyDictionary<string, PlayableCardData?> OpponentLanes;
    public readonly IReadOnlyDictionary<string, PlayableCardData?> OpponentQueueLanes;

    public readonly IReadOnlyList<PlayableCardData> PlayerHand;
    public readonly IReadOnlyList<CardData> RemainingDeck;

    public readonly int MaxItems;
    public readonly IReadOnlyList<ItemData> Items;

    public readonly TotemData? Totem;

    public readonly IReadOnlyDictionary<string, string>? SigilDescriptions;
    public readonly IReadOnlyDictionary<string, string>? TraitDescriptions;

    public readonly bool? IsOpponentSurrendering;

    [JsonProperty("Note")]
    private string? Note => Ijiraq.AnythingCouldBeIjiraq ? StateStrings.IjiraqPresent : null;

    // Excluded from JSON

    [JsonIgnore]
    public readonly IjiraqState Ijiraq;

    bool IGameState.ShouldFakeIjiraq(int deckIndex) => Ijiraq.FakeIjiraqsInDeck.Contains(deckIndex);

    private CombatState()
    {
        Ijiraq = new IjiraqState();

        Scale = LifeManager.Instance.Balance;
        Bones = ResourcesManager.Instance.PlayerBones;
        PlayerLives = RunState.Run.playerLives;

        PlayerLanes = BoardManager.Instance.playerSlots.ToIndexedDictionary(slot => PlayableCardData.Create(slot.Card, this))!;
        OpponentLanes = BoardManager.Instance.opponentSlots.ToIndexedDictionary(slot => PlayableCardData.Create(slot.Card, this))!;
        OpponentQueueLanes = BoardManager.Instance.opponentSlots.Select(BoardManager.Instance.GetCardQueuedForSlot).ToIndexedDictionary(card => PlayableCardData.Create(card, this))!;

        PlayerHand = DiskCardGame.PlayerHand.Instance.CardsInHand.Select(c => PlayableCardData.Create(c, this)).ToList();
        RemainingDeck = CardDrawPiles.Instance.Deck.Cards.Select((c, index) => CardData.Create(c, this, index)).ToList();

        MaxItems = RunState.Run.MaxConsumables;
        Items = ItemsManager.Instance.consumableSlots.Where(s => s.Item).Select(ItemData.Create).ToList();

        // (TurnManager.Instance.Opponent as Part1Opponent).totem
        Totem = TotemData.Create(Part1ItemsManager.Instance.Totems.FirstOrDefault());

        List<CardData> allCardsInPlay = [..PlayerLanes.Values, ..OpponentLanes.Values, ..OpponentQueueLanes.Values, ..PlayerHand, ..RemainingDeck];
        allCardsInPlay.RemoveAll(data => data == null);

        IEnumerable<SigilData> relevantSigils = allCardsInPlay.SelectMany(c => c.RelevantSigils);
        if (Totem != null) relevantSigils = relevantSigils.Append(Totem.Sigil);
        SigilDescriptions = relevantSigils.ToDescriptionDictionary().NullIfEmpty();
        TraitDescriptions = allCardsInPlay.SelectMany(c => c.Traits).Distinct().ToDescriptionDictionary().NullIfEmpty();

        IsOpponentSurrendering = TurnManager.Instance.Opponent.OfferingSurrender ? true : null;
    }

    // ["playerTotem"]
    // ["items"]
    // ["remainingSquirrels"] ?
    // ["enemyTotem"]
    // ["enemyLives"]
    // ["boons"]
    // ["bossAbilityDescription"] ?

    public sealed class IjiraqState
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
