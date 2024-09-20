using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using JetBrains.Annotations;

namespace NeuroInscryption.Communication.Data.State;

[PublicAPI]
public sealed class LoadoutState : IGameState
{
    public static LoadoutState Calculate() => new();

    public readonly List<CardData> PlayerDeck;
    public readonly int Teeth;

    // Excluded from JSON

    bool IGameState.ShouldFakeIjiraq(int deckIndex) => false;

    private LoadoutState()
    {
        PlayerDeck = RunState.Run.playerDeck.Cards.Select(c => CardData.Create(c, this)).ToList();
        Teeth = RunState.Run.currency;
    }
}
