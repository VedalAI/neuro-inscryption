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
        public List<CardDTO> PlayerHand => DiskCardGame.PlayerHand.Instance.CardsInHand.Select(CardDTO.Create).ToList();
    }

}
