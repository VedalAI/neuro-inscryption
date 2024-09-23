using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Data.State;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class LoadoutState : IGameState
{
    public static LoadoutState Calculate() => new();

    public readonly IReadOnlyList<CardData> PlayerDeck;
    public readonly int Teeth;

    public readonly TotemData? Totem;
    public readonly IReadOnlyList<TotemPieceData>? TotemPieces;

    public readonly IReadOnlyDictionary<string, string>? SigilDescriptions;
    public readonly IReadOnlyDictionary<string, string>? TraitDescriptions;

    [JsonExtensionData]
    public JObject? Context { get; private set; }

    bool IGameState.ShouldFakeIjiraq(int deckIndex) => false;

    private LoadoutState(object? decisionContext = null)
    {
        PlayerDeck = RunState.Run.playerDeck.Cards.Select(c => CardData.Create(c, this)).ToList();
        Teeth = RunState.Run.currency;

        Totem = TotemData.Create(Part1ItemsManager.Instance.Totems.FirstOrDefault());
        TotemPieces = RunState.Run.totemTops.Select(TotemPieceData.Create).Concat(RunState.Run.totemBottoms.Select(TotemPieceData.Create)).ToList().NullIfEmpty();

        List<SigilData> relevantSigils = PlayerDeck.SelectMany(c => c.RelevantSigils).ToList();
        if (Totem != null) relevantSigils.Add(Totem.Sigil);
        if (TotemPieces != null) relevantSigils.AddRange(TotemPieces.Select(p => p.Sigil).WhereNotNull().Select(SigilData.Create));
        SigilDescriptions = relevantSigils.ToDescriptionDictionary().NullIfEmpty();
        TraitDescriptions = PlayerDeck.SelectMany(c => c.Traits).Distinct().ToDescriptionDictionary().NullIfEmpty();

        SetContext(decisionContext);
    }

    public void SetContext<T>(T? decisionContext) where T : class
    {
        Context = decisionContext != null ? JObject.FromObject(decisionContext, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }) : null;
    }

    public void ClearContext()
    {
        Context = null;
    }
}
