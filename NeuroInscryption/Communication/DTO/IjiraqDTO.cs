using DiskCardGame;

namespace NeuroInscryption.Communication.DTO;

public class IjiraqDTO : CardDTO
{
    public IjiraqDTO(PlayableCard card) : base(CardLoader.GetCardByName("Ijiraq"), card)
    {
        Cost = card.Info.BonesCost > 0 ? $"{card.Info.BonesCost} bones (but won't consume them when played)" : $"{card.Info.BloodCost} blood";
        // TODO: Traits and Sigils on play stay on Ijiraq + Totem Sigils
    }
}
