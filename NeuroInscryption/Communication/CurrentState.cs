using System.Collections.Generic;
using DiskCardGame;

namespace NeuroInscryption.Communication;

public static class CurrentState
{
    public static Dictionary<string, object> Board => new()
    {
        ["scale"] = LifeManager.Instance.Balance,
        ["bones"] = ResourcesManager.Instance.PlayerBones,
        ["playerLives"] = RunState.Run.playerLives,
        // ["playerTotem"]
        // ["playerLanes"] = BoardManager.Instance.playerSlots
        // ["opponentLanes"] = BoardManager.Instance.opponentSlots
        // ["opponentQueueLanes"] = BoardManager.Instance.opponentQueueSlots
        // ["items"]
        // ["playerHand"]
        // ["remainingDeck"]
        // ["remainingSquirrels"] ?
        // ["enemyTotem"]
        // ["enemyLives"]
        // ["boons"]
        // ["isOpponentSurrendering"] ?
        // ["bossAbilityDescription"] ?
    };
}
