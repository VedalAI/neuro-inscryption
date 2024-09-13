using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data;

[PublicAPI]
public sealed class CardData
{
    private static readonly HashSet<Ability> _immediateSigils = [Ability.DrawRabbits, Ability.CreateDams, Ability.Tutor, Ability.DrawCopy, Ability.CorpseEater, Ability.DrawAnt, Ability.RandomConsumable, Ability.CreateEgg];
    private static readonly HashSet<Trait> _nonImmediateTraits = [Trait.Ant, Trait.Pelt, Trait.Uncuttable, Trait.Giant, Trait.KillsSurvivors, Trait.Goat, Trait.Lice];
    private static readonly HashSet<SpecialTriggeredAbility> _nonImmediateAbilities = [SpecialTriggeredAbility.Cat, SpecialTriggeredAbility.JerseyDevil, SpecialTriggeredAbility.Daus, SpecialTriggeredAbility.Ouroboros];

    public readonly string Name;

    public readonly int? Health;

    public readonly string? Power;

    [JsonProperty("Sigils")]
    private List<string>? SigilNames => Sigils.Select(d => d.Name).ToList().NullIfEmpty();

    public readonly bool? IsPlayable;

    public readonly string? Cost;

    public readonly bool? CanBeSacrificed;

    public readonly int? TimesSacrificed;

    public readonly string? Tribes;

    [JsonProperty("Traits")]
    private List<string>? TraitNames => Traits.Select(d => d.Name).ToList().NullIfEmpty();

    [JsonProperty]
    private string? Note => _maybeIjiraq ? string.Format(CardStrings.IjiraqNarrowedDown, TargetCardInfo.DisplayedNameEnglish) : null;

    // Excluded from JSON

    [JsonIgnore]
    public readonly List<SigilData> Sigils;

    [JsonIgnore]
    public readonly List<SigilData> RelevantSigils;

    [JsonIgnore]
    public readonly List<TraitData> Traits;

    [JsonIgnore]
    public readonly CardInfo TargetCardInfo;

    [JsonIgnore]
    public readonly PlayableCard? TargetCard;

    private readonly GameState.Board _state;

    private readonly bool _maybeIjiraq;

    private CardData(CardInfo cardInfo, GameState.Board state, int? deckIndex = null)
    {
        _state = state;
        TargetCardInfo = cardInfo;

        if (TargetCardInfo.name == "!STATIC!GLITCH")
        {
            Name = "Glitched Card";
            Sigils = [];
            RelevantSigils = [];
            Traits = [];
            return;
        }

        if (deckIndex != null && _state.Ijiraq.FakeIjiraqsInDeck.Contains(deckIndex.Value))
        {
            TargetCardInfo = CardLoader.GetCardByName("Ijiraq");
        }

        Name = TargetCardInfo.DisplayedNameEnglish;
        Health = TargetCardInfo.Health;
        Power = CalculatePower();
        Sigils = GetSigils(false);
        RelevantSigils = GetSigils(true);
        Cost = TargetCardInfo.BonesCost > 0 ? string.Format(CardStrings.BonesCost, TargetCardInfo.BonesCost) : string.Format(CardStrings.BloodCost, TargetCardInfo.BloodCost);
        CanBeSacrificed = TargetCardInfo.Sacrificable || TargetCardInfo.HasAbility(Ability.TripleBlood);
        Tribes = TargetCardInfo.tribes.Count == 0 ? null : string.Join(CardStrings.InfoSeparator, TargetCardInfo.tribes);
        Traits = GetTraits();
    }

    private CardData(PlayableCard card, GameState.Board state) : this(card.Info, state)
    {
        TargetCard = card;

        Name = $"{TargetCard.Info.DisplayedNameEnglish}{(TargetCard.OpponentCard ? null : $" ({(TargetCard.InHand ? 12 + PlayerHand.Instance.CardsInHand.IndexOf(TargetCard) : BoardManager.Instance.CardsOnBoard.IndexOf(TargetCard))})")}";
        Power = CalculatePower();
        IsPlayable = !TargetCard.InHand ? null : TargetCard.CanPlay();

        if (!TargetCard.InHand) Cost = null;
        if (TargetCard.OpponentCard) CanBeSacrificed = null;

        if (TargetCard.TryGetSpecialAbility(SpecialTriggeredAbility.Cat, out Cat? cat)) TimesSacrificed = cat.sacrificeCount;
        if (TargetCard.TryGetSpecialAbility(SpecialTriggeredAbility.JerseyDevil, out JerseyDevil? child13)) TimesSacrificed = child13.sacrificeCount;

        if (_state.Ijiraq.ExposableIjiraqs.Contains(TargetCard))
        {
            CardInfo ijiraq = CardLoader.GetCardByName("Ijiraq");

            Name = ijiraq.DisplayedNameEnglish;
            Health = ijiraq.Health;
            Power = ijiraq.Attack.ToString();
            if (TargetCard.Info.BonesCost > 0) Cost = string.Format(CardStrings.IjiraqBonesCost, TargetCard.Info.BonesCost);
            CanBeSacrificed = true;
            Tribes = null;

            Sigils.RemoveAll(data => !_immediateSigils.Contains(data.Sigil));
            Sigils.Add(Ability.PreventAttack);

            Traits.RemoveAll(data => data.TryGetTrait(out Trait trait) && _nonImmediateTraits.Contains(trait));
            Traits.RemoveAll(data => data.TryGetAbility(out SpecialTriggeredAbility ability) && _nonImmediateAbilities.Contains(ability));
            Traits.Add(SpecialTriggeredAbility.Shapeshifter!);

            // TODO: Totem Sigils
        }
        else if (_state.Ijiraq.SuspiciousCardsInHand.Contains(TargetCard))
        {
            _maybeIjiraq = true;
        }
    }

    [return: NotNullIfNotNull(nameof(cardInfo))]
    public static CardData? CreateFromDeck(CardInfo? cardInfo, int deckIndex, GameState.Board state) => cardInfo != null ? new CardData(cardInfo, state) : null;

    [return: NotNullIfNotNull(nameof(card))]
    public static CardData? Create(PlayableCard? card, GameState.Board state) => card != null ? new CardData(card, state) : null;

    public string GetDescription()
    {
        return string.Format(CardStrings.Description,
            Name,
            Power,
            Health,
            SigilNames != null ? CardStrings.InfoSeparator + string.Join(CardStrings.InfoSeparator, SigilNames) : "",
            _maybeIjiraq ? CardStrings.InfoSeparator + CardStrings.IjiraqNarrowedDownShortDescription : "",
            Cost
        );
    }

    private string CalculatePower()
    {
        foreach (SpecialTriggeredAbility ability in TargetCardInfo.SpecialAbilities)
        {
            switch (ability)
            {
                case SpecialTriggeredAbility.Ant:
                    return string.Format(PowerStrings.Ant, BoardManager.Instance.playerSlots.Where(s => s.Card && s.Card.Info.traits.Contains(Trait.Ant)));

                case SpecialTriggeredAbility.Lammergeier:
                    return string.Format(PowerStrings.Lammergeier, _state.Bones, _state.Bones / 2);

                case SpecialTriggeredAbility.SacrificesThisTurn:
                    return string.Format(PowerStrings.SacrificesThisTurn, BoardManager.Instance.SacrificesMadeThisTurn);

                case SpecialTriggeredAbility.Mirror:
                    return string.Format(TargetCard != null && !TargetCard.InHand ? PowerStrings.MirrorInPlay : PowerStrings.Mirror, TargetCard?.Attack);

                case SpecialTriggeredAbility.BellProximity:
                    return string.Format(TargetCard != null && !TargetCard.InHand ? PowerStrings.BellProximityInPlay : PowerStrings.BellProximity, TargetCard?.Attack);

                case SpecialTriggeredAbility.CardsInHand:
                    return string.Format(PowerStrings.CardsInHand, _state.PlayerHand.Count);
            }
        }

        return TargetCardInfo.Attack.ToString();
    }

    private List<SigilData> GetSigils(bool includeTangential)
    {
        List<SigilData> result = [];

        result.AddRange(TargetCardInfo.Abilities.Select(SigilData.Create));

        if (includeTangential)
        {
            if (TargetCardInfo.specialAbilities.Contains(SpecialTriggeredAbility.JerseyDevil)) result.Add(Ability.Flying);
            if (TargetCardInfo.specialAbilities.Contains(SpecialTriggeredAbility.TrapSpawner)) result.AddRange([Ability.SteelTrap, Ability.Reach]);
            if (TargetCardInfo.Abilities.Contains(Ability.BeesOnHit)) result.Add(Ability.Flying);
            if (TargetCardInfo.Abilities.Contains(Ability.Reach)) result.Add(Ability.Flying);
        }

        result = result.Distinct().ToList();
        result.Sort();
        return result;
    }

    private List<TraitData> GetTraits()
    {
        List<TraitData> result = [];

        result.AddRange(TargetCardInfo.traits.Select(TraitData.Create).Where(t => t != null)!);
        result.AddRange(TargetCardInfo.specialAbilities.Select(TraitData.Create).Where(t => t != null)!);
        if (TargetCardInfo.name.ToLowerInvariant().Contains("bell")) result.Add(TraitData.Create(TraitStrings.Chime));

        if (GameFlowManager.Instance.CurrentGameState == DiskCardGame.GameState.CardBattle)
        {
            // boon lord and campfire stuff we don't care about while in a card battle
            result.RemoveAll(t => t == Trait.KillsSurvivors || t == Trait.Goat || t == Trait.Pelt);
        }

        result.Sort();
        return result;
    }
}
