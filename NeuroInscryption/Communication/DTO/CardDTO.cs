using DiskCardGame;

namespace NeuroInscryption.Communication.DTO;

public sealed class CardDTO
{
    public readonly string Name;
    public readonly int Health;
    public readonly string Power;
    // sigils
    public readonly bool? IsPlayable;
    public readonly string Cost;
    public readonly bool? CanBeSacrificed;
    public readonly string Tribes;
    // public readonly string Traits = GetTraits(card);
    // specialAbilities
    public readonly string Note;

    public CardDTO(CardInfo cardInfo)
    {
        Name = cardInfo.DisplayedNameEnglish;
        Health = cardInfo.Health;
        Power = CalculatePower(cardInfo);
        Cost = cardInfo.BonesCost > 0 ? $"{cardInfo.BonesCost} bones" : $"{cardInfo.BloodCost} blood";
        CanBeSacrificed = cardInfo.Sacrificable || cardInfo.HasAbility(Ability.TripleBlood);
        Tribes = cardInfo.tribes.Count == 0 ? null : string.Join(", ", cardInfo.tribes);
    }

    public CardDTO(CardInfo cardInfo, PlayableCard playableCard) : this(cardInfo)
    {
        Name = $"{cardInfo.DisplayedNameEnglish} {(playableCard.OpponentCard ? null : $"({(playableCard.InHand ? PlayerHand.Instance.CardsInHand.IndexOf(playableCard) : BoardManager.Instance.CardsOnBoard.IndexOf(playableCard))})")}";
        Power = CalculatePower(cardInfo, playableCard);
        IsPlayable = !playableCard.InHand ? null : playableCard.CanPlay();

        if (!playableCard.InHand) Cost = null;
        if (playableCard.OpponentCard) CanBeSacrificed = null;

        if (GameState.Ijiraq.Current.PotentialIjiraqsInHand.Contains(playableCard)) Note = "Careful! This card or another identical card in your hand is a disguised Ijiraq!";
    }

    public CardDTO(PlayableCard card) : this(card.Info, card) { }

    private static string CalculatePower(CardInfo cardInfo, PlayableCard cardInPlay = null)
    {
        /*
            Cat => Transforms into Undead Cat after being sacrified 9 times
            Ant => Power is equal to number of Ants
            RandomCard => Transforms into a random card when drawn
            Lammergeier => Power is equal to half the number of bones
            --- TalkingCardChooser,
            PackMule => Gives 4 cards in hand when this card dies
            Mirror => Power is equal to the power of the card opposite this one
            BellProximity => Power ranges from 4 to 1, depending on which lane you place it in and how close it is to the bell
            --- CagedWolf,
            TrapSpawner => Spawns a Leaping Trap in its place when it dies
            GiantCard => Increases in power by 1 for each turn after you run out of cards
            --- GiantMoon,
            --- GreenMage,
            JerseyDevil => Transforms into Hungry Child after being sacrified 13 times
            Daus => If a card with the Chime trait is attacked, this card will attack the attacker
            --- BountyHunter
            --- BrokenCoinLeft,
            --- BrokenCoinRight,
            CardsInHand => Power is equal to the number of cards in your hand
            Ouroboros => Health and power increases by 1 permanently each time it dies or is sacrificed
            Shapeshifter => This card disguises itself as another card in the deck and only reveals itself after it is played // TODO Disguise Ijiraq in hand
            SacrificesThisTurn,
            SpawnLice,
            GiantShip,
         */

        foreach (SpecialTriggeredAbility ability in cardInfo.specialAbilities)
        {
            switch (ability)
            {
                case SpecialTriggeredAbility.Ant:
                    return $"Equal to the number of ants on your side of the board (currently";// TODO {CurrentState.Board.PlayerLanes.Count(c => c != null && c has trait ant)})";

                case SpecialTriggeredAbility.Lammergeier:
                    return $"Equal to half the number of bones you have (currently {GameState.Board.Current.Bones} bones, meaning {GameState.Board.Current.Bones / 2} damage)";

                case SpecialTriggeredAbility.SacrificesThisTurn:
                    return $"Equal to the number of cards you've sacrificed this turn (currently {BoardManager.Instance.SacrificesMadeThisTurn})";

                case SpecialTriggeredAbility.Mirror:
                {
                    string result = "Equal to the power of the card opposite this one";
                    if (cardInPlay && !cardInPlay.InHand) result += $" (currently {cardInPlay.Attack})";
                    return result;
                }

                case SpecialTriggeredAbility.BellProximity:
                {
                    string result = "Ranges from 4 to 1, depending on which lane you place it in. Lane 0 means 4 attack, lane 1 means 3 attack, lane 2 means 2 attack and lane 3 means 1 attack.";
                    if (cardInPlay && !cardInPlay.InHand) result += $" (currently {cardInPlay.Attack})";
                    return result;
                }

                case SpecialTriggeredAbility.CardsInHand:
                    return $"Equal to the number of cards in your hand (currently {GameState.Board.Current.PlayerHand.Count})";
            }
        }

        return cardInfo.Attack.ToString();
    }
}
