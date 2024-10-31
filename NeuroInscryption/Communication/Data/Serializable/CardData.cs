using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Strings;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

// TODO: privatize json properties
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class CardData
{
    [JsonProperty(Order = 0)]
    public string DisplayName { get; protected set; }

    [JsonProperty(Order = 10)]
    public int? Health { get; protected set; }

    [JsonProperty(Order = 20)]
    public string? Power { get; protected set; }

    [JsonProperty("Sigils", Order = 30)]
    protected List<string>? SigilNames => Sigils.Select(d => d.Name).ToList().NullIfEmpty();

    [JsonProperty(Order = 40)]
    public string? Cost { get; protected set; }

    [JsonProperty(Order = 50)]
    public bool? CanBeSacrificed { get; protected set; }

    [JsonProperty(Order = 60)]
    public string? Tribes { get; protected set; }

    [JsonProperty("Traits", Order = 70)]
    private List<string>? TraitNames => Traits.Select(d => d.Name).ToList().NullIfEmpty();

    [JsonProperty(Order = 80)]
    public bool? IsRare { get; protected set; }

    // TODO: Is empowered
    // TODO: about to be grabbed by hook
    // TODO: about to be cannoned
    // TODO: empty slot about to be cannoned

    // Excluded from JSON

    [JsonIgnore]
    public List<SigilData> Sigils { get; protected set; }

    [JsonIgnore]
    public List<SigilData> RelevantSigils { get; protected set; }

    [JsonIgnore]
    public List<TraitData> Traits { get; protected set; }

    [JsonIgnore]
    public CardInfo TargetCardInfo { get; private set; }
    [JsonIgnore]
    protected string DescCost => Cost == CardStrings.FreeCost ? "no cost" : CardStrings.CostPrefix + Cost;

    protected CardData(CardInfo cardInfo, IGameState? state = null, int? deckIndex = null)
    {
        TargetCardInfo = cardInfo;

        if (TargetCardInfo.name == "!STATIC!GLITCH")
        {
            DisplayName = "Glitched Card";
            Sigils = [];
            RelevantSigils = [];
            Traits = [];
            return;
        }

        if (deckIndex != null && state?.ShouldFakeIjiraq(deckIndex.Value) == true)
        {
            TargetCardInfo = CardLoader.GetCardByName("Ijiraq");
        }

        DisplayName = TargetCardInfo.DisplayedNameEnglish;
        Health = TargetCardInfo.Health;
        Power = GetPowerInDeck();
        Sigils = GetSigils(false);
        RelevantSigils = GetSigils(true);
        Cost = GetCost();
        CanBeSacrificed = TargetCardInfo.Sacrificable || TargetCardInfo.HasAbility(Ability.TripleBlood);
        Tribes = TargetCardInfo.tribes.Count == 0 ? null : string.Join(CardStrings.InfoSeparator, TargetCardInfo.tribes);
        Traits = GetTraits();
        IsRare = TargetCardInfo.metaCategories.Contains(CardMetaCategory.Rare) ? true : null;
    }

    [return: NotNullIfNotNull(nameof(cardInfo))]
    public static CardData? Create(CardInfo? cardInfo, IGameState? state = null, int? deckIndex = null) => cardInfo != null ? new CardData(cardInfo, state, deckIndex) : null;

    public virtual string GetDescription()
    {
        return StringFormatter.Format(CardStrings.Description,
            DisplayName,
            Power,
            Health,
            SigilNames != null ? CardStrings.InfoSeparator + string.Join(CardStrings.InfoSeparator, SigilNames) : "",
            "",
            DescCost
        );
    }

    private string GetPowerInDeck()
    {
        foreach (SpecialTriggeredAbility ability in TargetCardInfo.SpecialAbilities)
        {
            switch (ability)
            {
                case SpecialTriggeredAbility.Ant:
                    return PowerStrings.Ant;

                case SpecialTriggeredAbility.Lammergeier:
                    return PowerStrings.Lammergeier;

                case SpecialTriggeredAbility.SacrificesThisTurn:
                    return PowerStrings.SacrificesThisTurn;

                case SpecialTriggeredAbility.Mirror:
                    return PowerStrings.Mirror;

                case SpecialTriggeredAbility.BellProximity:
                    return PowerStrings.BellProximity;

                case SpecialTriggeredAbility.CardsInHand:
                    return PowerStrings.CardsInHand;
            }
        }

        return TargetCardInfo.Attack.ToString();
    }

    private string GetCost()
    {
        if (TargetCardInfo.BloodCost > 0) return StringFormatter.Format(CardStrings.BloodCost, TargetCardInfo.BloodCost);
        if (TargetCardInfo.BonesCost > 0) return StringFormatter.Format(CardStrings.BonesCost, TargetCardInfo.BonesCost);
        return CardStrings.FreeCost;
    }

    private List<SigilData> GetSigils(bool includeTangential)
    {
        List<Ability> abilities = [];

        abilities.AddRange(TargetCardInfo.Abilities);

        if (includeTangential)
        {
            if (TargetCardInfo.specialAbilities.Contains(SpecialTriggeredAbility.JerseyDevil)) abilities.Add(Ability.Flying);
            if (TargetCardInfo.specialAbilities.Contains(SpecialTriggeredAbility.TrapSpawner)) abilities.AddRange([Ability.SteelTrap, Ability.Reach]);
            if (TargetCardInfo.Abilities.Contains(Ability.BeesOnHit)) abilities.Add(Ability.Flying);
            if (TargetCardInfo.Abilities.Contains(Ability.Reach)) abilities.Add(Ability.Flying);
        }

        List<SigilData> result = abilities.Distinct().Select(SigilData.Create).ToList();
        result.Sort();
        return result;
    }

    private List<TraitData> GetTraits()
    {
        List<TraitData> result = [];

        result.AddRange(TargetCardInfo.traits.Select(TraitData.Create).WhereNotNull());
        result.AddRange(TargetCardInfo.specialAbilities.Select(TraitData.Create).WhereNotNull());
        if (TargetCardInfo.name.ToLowerInvariant().Contains("bell")) result.Add(TraitData.Create(TraitStrings.Chime));
        if (TargetCardInfo.name == "Hodag") result.Add(TraitData.Create(TraitStrings.Hodag));
        if (TargetCardInfo.name == "BaitBucket") result.Add(TraitData.Create(TraitStrings.BaitBucket));
        if (TargetCardInfo.name == "!GIANTCARD_SHIP") result.Add(TraitData.Create(TraitStrings.Limoncello));

        if (GameFlowManager.Instance.CurrentGameState == GameState.CardBattle)
        {
            // boon lord and campfire stuff we don't care about while in a card battle
            result.RemoveAll(t => t.TryGetTrait(out Trait trait) && trait is Trait.KillsSurvivors or Trait.Goat or Trait.Pelt);
        }

        result.Sort();
        return result;
    }
}
