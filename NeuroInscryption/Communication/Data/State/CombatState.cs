using System;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BoonData = NeuroInscryption.Communication.Data.Serializable.BoonData;
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
    public IReadOnlyList<CardData>? RemainingDeck => _contextInterface is { IncludeDeck: false } ? null : _remainingDeck;
    public string RemainingSquirrels;
    public readonly IReadOnlyList<BoonData>? Boons;

    public readonly int MaxItems;
    public readonly IReadOnlyList<ItemData> Items;

    public readonly TotemData? Totem;

    public readonly int? OpponentLives;
    public readonly TotemData? OpponentTotem;
    public readonly string? BossAbilityDescription; // TODO
    public readonly bool? IsOpponentSurrendering;

    // TODO: Follow up nodes
    // TODO: Oil paninting puzzle

    [JsonProperty("Note")]
    private string? Note => Ijiraq.AnythingCouldBeIjiraq ? CardStrings.IjiraqPresent : null;

    public readonly IReadOnlyDictionary<string, string>? SigilDescriptions;
    public readonly IReadOnlyDictionary<string, string>? TraitDescriptions;

    [JsonExtensionData]
    public JObject? Context { get; private set; }

    // Excluded from JSON

    [JsonIgnore]
    public readonly IjiraqState Ijiraq;

    [JsonIgnore]
    private readonly IReadOnlyList<CardData> _remainingDeck;

    [JsonIgnore]
    private ICombatContext? _contextInterface;

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
        _remainingDeck = CardDrawPiles.Instance.Deck.Cards.Select((c, index) => CardData.Create(c, this, index)).ToList();
        RemainingSquirrels = CardDrawPiles3D.Instance.SideDeck.Cards.Count > 4 ? "Many" : CardDrawPiles3D.Instance.SideDeck.Cards.Count.ToString();
        Boons = RunState.Run.playerDeck.Boons.Select(BoonData.Create).ToList().NullIfEmpty();

        MaxItems = RunState.Run.MaxConsumables;
        Items = ItemsManager.Instance.consumableSlots.Where(s => s.Item).Select(ItemData.Create).ToList();

        Totem = TotemData.Create(Part1ItemsManager.Instance.Totems.FirstOrDefault());

        Part1Opponent opponent = (Part1Opponent) TurnManager.Instance.Opponent;
        OpponentLives = opponent.StartingLives > 1 ? opponent.NumLives : null;
        OpponentTotem = TotemData.Create(opponent.totem);
        IsOpponentSurrendering = TurnManager.Instance.Opponent.OfferingSurrender ? true : null;

        List<CardData> allCardsInPlay = [..PlayerLanes.Values, ..OpponentLanes.Values, ..OpponentQueueLanes.Values, ..PlayerHand, .._remainingDeck];
        allCardsInPlay.RemoveAll(data => data == null);

        IEnumerable<SigilData> relevantSigils = allCardsInPlay.SelectMany(c => c.RelevantSigils);
        if (Totem != null) relevantSigils = relevantSigils.Append(SigilData.Create(Totem.Sigil));
        SigilDescriptions = relevantSigils.ToDescriptionDictionary().NullIfEmpty();
        TraitDescriptions = allCardsInPlay.SelectMany(c => c.Traits).Distinct().ToDescriptionDictionary().NullIfEmpty();
    }

    public void SetContext(ICombatContext decisionContext)
    {
        Context = JObject.FromObject(decisionContext, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
        _contextInterface = decisionContext;
    }

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
