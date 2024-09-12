using DiskCardGame;

namespace NeuroInscryption.Communication.Data;

public class IjiraqCardData : CardData
{
    public IjiraqCardData(PlayableCard card) : base(CardLoader.GetCardByName("Ijiraq"), card)
    {
        Cost = card.Info.BonesCost > 0 ? $"{card.Info.BonesCost} bones (but won't consume them when played)" : $"{card.Info.BloodCost} blood";
        // TODO: Traits and Sigils on play stay on Ijiraq + Totem Sigils
    }
}
