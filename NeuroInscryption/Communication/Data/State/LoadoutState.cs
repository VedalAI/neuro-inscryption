using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Data.State;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class LoadoutState : IGameState
{
    public static LoadoutState Calculate<T>(T? decisionContext) where T : class => new(decisionContext);
    public static LoadoutState Calculate() => new();

    public readonly IReadOnlyList<CardData> PlayerDeck;
    public readonly int Teeth;

    [JsonExtensionData]
    public readonly JObject? Context;

    // Excluded from JSON

    bool IGameState.ShouldFakeIjiraq(int deckIndex) => false;

    private LoadoutState(object? decisionContext = null)
    {
        PlayerDeck = RunState.Run.playerDeck.Cards.Select(c => CardData.Create(c, this)).ToList();
        Teeth = RunState.Run.currency;
        Context = decisionContext != null ? JObject.FromObject(decisionContext) : null;
    }
}
