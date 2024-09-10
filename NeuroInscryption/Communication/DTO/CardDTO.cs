using DiskCardGame;

namespace NeuroInscryption.Communication.DTO;

public readonly struct CardDTO(PlayableCard card)
{
    public readonly string Name = $"{card.Info.DisplayedNameEnglish} ({(card.OpponentCard ? null : card.InHand ? PlayerHand.Instance.CardsInHand.IndexOf(card) : BoardManager.Instance.CardsOnBoard.IndexOf(card))})";
    public readonly int Health = card.Info.Health;
    // public string Power = card.InHand ? card.Attack
    // sigils
    public readonly bool? IsPlayable = !card.InHand ? null : card.CanPlay();
    public readonly string Cost = !card.InHand ? null : card.Info.BonesCost > 0 ? $"{card.Info.BonesCost} bones" : $"{card.Info.BloodCost} blood";
    public readonly bool? CanBeSacrificed = card.OpponentCard ? null : card.Info.Sacrificable || card.HasAbility(Ability.TripleBlood);
    public readonly string Tribes = card.Info.tribes.Count == 0 ? null : string.Join(", ", card.Info.tribes);
    // public readonly string Traits = GetTraits(card);

    public static CardDTO Create(PlayableCard card) => new(card);
}
