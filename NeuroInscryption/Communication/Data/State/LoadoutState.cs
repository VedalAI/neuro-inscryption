using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BoonData = NeuroInscryption.Communication.Data.Serializable.BoonData;
using ItemData = NeuroInscryption.Communication.Data.Serializable.ItemData;

namespace NeuroInscryption.Communication.Data.State;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class LoadoutState : IGameState
{
    public static LoadoutState Calculate() => new();

    public readonly IReadOnlyList<CardData> PlayerDeck;
    public readonly IReadOnlyList<BoonData>? Boons;

    public readonly int Teeth;

    public readonly int MaxItems;
    public readonly IReadOnlyList<ItemData> Items;

    public readonly TotemData? Totem;
    public readonly IReadOnlyList<TotemPieceData>? TotemPieces;

    public readonly IReadOnlyDictionary<string, string>? SigilDescriptions;
    public readonly IReadOnlyDictionary<string, string>? TraitDescriptions;

    [JsonExtensionData]
    public JObject? Context { get; private set; }

    [JsonIgnore]
    private ILoadoutContext? _contextInterface;

    bool IGameState.ShouldFakeIjiraq(int deckIndex) => false;

    private LoadoutState(object? decisionContext = null)
    {
        PlayerDeck = RunState.Run.playerDeck.Cards.Select(c => CardData.Create(c, this)).ToList();
        Boons = RunState.Run.playerDeck.Boons.Select(BoonData.Create).ToList().NullIfEmpty();

        Teeth = RunState.Run.currency;

        MaxItems = RunState.Run.MaxConsumables;
        Items = ItemsManager.Instance.consumableSlots.Where(s => s.Item).Select(ItemData.Create).ToList();

        Totem = TotemData.Create(Part1ItemsManager.Instance.Totems.FirstOrDefault());
        TotemPieces = RunState.Run.totemTops.Select(TotemPieceData.Create).Concat(RunState.Run.totemBottoms.Select(TotemPieceData.Create)).ToList().NullIfEmpty();

        List<SigilData> relevantSigils = PlayerDeck.SelectMany(c => c.RelevantSigils).ToList();
        List<TraitData> relevantTraits = PlayerDeck.SelectMany(c => c.Traits).ToList();
        if (Totem != null) relevantSigils.Add(Totem.Sigil);
        if (TotemPieces != null) relevantSigils.AddRange(TotemPieces.Select(p => p.Sigil).WhereNotNull().Select(SigilData.Create));
        if (_contextInterface != null)
        {
            relevantSigils.AddRange(_contextInterface.RelevantSigils);
            relevantTraits.AddRange(_contextInterface.RelevantTraits);
        }

        SigilDescriptions = relevantSigils.ToDescriptionDictionary().NullIfEmpty();
        TraitDescriptions = relevantTraits.ToDescriptionDictionary().NullIfEmpty();

        SetContext((ILoadoutContext?) decisionContext);
    }

    public void SetContext<T>(T? decisionContext) where T : class, ILoadoutContext
    {
        Context = decisionContext != null ? JObject.FromObject(decisionContext, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }) : null;
        _contextInterface = decisionContext;
    }

    public void ClearContext()
    {
        Context = null;
    }
}
