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
    public static LoadoutState CalculateWithContext<T>(T? decisionContext) where T : class => new(decisionContext);
    public static LoadoutState Calculate() => new();

    public IReadOnlyList<CardData> PlayerDeck { get; }
    public int Teeth { get; }

    [JsonExtensionData]
    public JObject? Context { get; private set; }

    // Excluded from JSON

    bool IGameState.ShouldFakeIjiraq(int deckIndex) => false;

    private LoadoutState(object? decisionContext = null)
    {
        PlayerDeck = RunState.Run.playerDeck.Cards.Select(c => CardData.Create(c, this)).ToList();
        Teeth = RunState.Run.currency;
        SetContext(decisionContext);
    }

    public void SetContext<T>(T? decisionContext) where T : class
    {
        Context = decisionContext != null ? JObject.FromObject(decisionContext, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore }) : null;
    }
}
