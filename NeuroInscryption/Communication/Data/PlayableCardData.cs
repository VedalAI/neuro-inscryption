using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PlayableCardData : CardData
{
    [JsonProperty(Order = 1)]
    public string Identifier { get; private set; }

    [JsonProperty(Order = 41)]
    public bool? IsPlayable { get; private set; }

    [JsonProperty(Order = 51)]
    public int? TimesSacrificed { get; private set; }

    [JsonProperty(Order = 100)]
    private string? Note => _maybeIjiraq ? string.Format(CardStrings.IjiraqNarrowedDown, TargetCardInfo.DisplayedNameEnglish) : null;

    // Excluded from JSON

    [JsonIgnore]
    public PlayableCard TargetCard { get; private set; }

    private readonly CombatState _state;

    private readonly bool _maybeIjiraq;

    private PlayableCardData(PlayableCard card, CombatState state) : base(card.Info, state)
    {
        _state = state;
        TargetCard = card;

        DisplayName = TargetCardInfo.DisplayedNameEnglish;
        Identifier = GetIdentifier(TargetCardInfo.DisplayedNameEnglish);
        Power = GetPowerInHandOrOnBoard();
        IsPlayable = !TargetCard.InHand ? null : TargetCard.CanPlay();

        if (!TargetCard.InHand) Cost = null;
        if (TargetCard.OpponentCard) CanBeSacrificed = null;

        if (TargetCard.TryGetSpecialAbility(SpecialTriggeredAbility.Cat, out Cat? cat)) TimesSacrificed = cat.sacrificeCount;
        if (TargetCard.TryGetSpecialAbility(SpecialTriggeredAbility.JerseyDevil, out JerseyDevil? child13)) TimesSacrificed = child13.sacrificeCount;

        if (_state.Ijiraq.ExposableIjiraqs.Contains(TargetCard))
        {
            CardInfo ijiraq = CardLoader.GetCardByName("Ijiraq");

            DisplayName = ijiraq.DisplayedNameEnglish;
            Identifier = GetIdentifier(ijiraq.DisplayedNameEnglish);
            Health = ijiraq.Health;
            Power = ijiraq.Attack.ToString();
            if (TargetCard.Info.BonesCost > 0) Cost = PluralFormatProvider.Format(CardStrings.BonesCost + CardStrings.IjiraqBonesCostSuffix, TargetCard.Info.BonesCost);
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

    [return: NotNullIfNotNull(nameof(card))]
    public static PlayableCardData? Create(PlayableCard? card, CombatState state) => card != null ? new PlayableCardData(card, state) : null;

    public string GetDescription()
    {
        return string.Format(CardStrings.Description,
            DisplayName,
            Power,
            Health,
            SigilNames != null ? CardStrings.InfoSeparator + string.Join(CardStrings.InfoSeparator, SigilNames) : "",
            _maybeIjiraq ? CardStrings.InfoSeparator + CardStrings.IjiraqNarrowedDownShortDescription : "",
            Cost
        );
    }

    private string GetPowerInHandOrOnBoard()
    {
        foreach (SpecialTriggeredAbility ability in TargetCardInfo.SpecialAbilities)
        {
            switch (ability)
            {
                case SpecialTriggeredAbility.Ant:
                    return PowerStrings.Ant + string.Format(PowerStrings.InPlaySuffix, BoardManager.Instance.playerSlots.Where(s => s.Card && s.Card.Info.traits.Contains(Trait.Ant)));

                case SpecialTriggeredAbility.Lammergeier:
                    return PowerStrings.Lammergeier + string.Format(PowerStrings.LammergeierInPlaySuffix, _state.Bones, _state.Bones / 2);

                case SpecialTriggeredAbility.SacrificesThisTurn:
                    return PowerStrings.SacrificesThisTurn + string.Format(PowerStrings.InPlaySuffix, BoardManager.Instance.SacrificesMadeThisTurn);

                case SpecialTriggeredAbility.Mirror:
                    if (TargetCard.InHand) return PowerStrings.Mirror;
                    return PowerStrings.Mirror + string.Format(PowerStrings.InPlaySuffix, TargetCard.Attack);

                case SpecialTriggeredAbility.BellProximity:
                    if (TargetCard.InHand) return PowerStrings.BellProximity;
                    return PowerStrings.BellProximity + string.Format(PowerStrings.InPlaySuffix, TargetCard.Attack);

                case SpecialTriggeredAbility.CardsInHand:
                    return PowerStrings.CardsInHand + string.Format(PowerStrings.InPlaySuffix, _state.PlayerHand.Count);
            }
        }

        return TargetCardInfo.Attack.ToString();
    }

    private string GetIdentifier(string displayName)
    {
        StringBuilder sb = new(displayName);

        if (TargetCard.OpponentCard) return sb.ToString();

        if (TargetCard.InHand)
        {
            sb.AppendFormat(CardStrings.GenericDiscriminator, PlayerHand.Instance.CardsInHand.IndexOf(TargetCard));
        }
        else
        {
            sb.AppendFormat(CardStrings.OnBoardDiscriminator, BoardManager.Instance.playerSlots.FindIndex(slot => slot.Card == TargetCard));
        }

        return sb.ToString();
    }

    private static readonly HashSet<Ability> _immediateSigils = [Ability.DrawRabbits, Ability.CreateDams, Ability.Tutor, Ability.DrawCopy, Ability.CorpseEater, Ability.DrawAnt, Ability.RandomConsumable, Ability.CreateEgg];
    private static readonly HashSet<Trait> _nonImmediateTraits = [Trait.Ant, Trait.Pelt, Trait.Uncuttable, Trait.Giant, Trait.KillsSurvivors, Trait.Goat, Trait.Lice];
    private static readonly HashSet<SpecialTriggeredAbility> _nonImmediateAbilities = [SpecialTriggeredAbility.Cat, SpecialTriggeredAbility.JerseyDevil, SpecialTriggeredAbility.Daus, SpecialTriggeredAbility.Ouroboros];
}
