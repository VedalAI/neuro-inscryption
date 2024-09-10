using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.DTO;

namespace NeuroInscryption.Communication;

public static class CurrentState
{
    public static readonly BoardState Board = new();

    public class BoardState
    {
        public int Scale => LifeManager.Instance.Balance;
        public int Bones => ResourcesManager.Instance.PlayerBones;
        public int PlayerLives => RunState.Run.playerLives;
        public List<CardDTO> PlayerLanes => BoardManager.Instance.playerSlots.Select(CardDTO.Create).ToList();
        public List<CardDTO> OpponentLanes => BoardManager.Instance.opponentSlots.Select(CardDTO.Create).ToList();
        public List<CardDTO> OpponentQueueLanes => BoardManager.Instance.opponentSlots.Select(BoardManager.Instance.GetCardQueuedForSlot).Select(CardDTO.Create).ToList();
        public List<CardDTO> PlayerHand => DiskCardGame.PlayerHand.Instance.CardsInHand.Select(CardDTO.Create).ToList();

        // ["playerTotem"]
        // ["items"]
        // ["remainingDeck"]
        // ["remainingSquirrels"] ?
        // ["enemyTotem"]
        // ["enemyLives"]
        // ["boons"]
        // ["isOpponentSurrendering"] ?
        // ["bossAbilityDescription"] ?
    }
}
