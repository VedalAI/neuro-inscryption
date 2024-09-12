namespace NeuroInscryption.Communication.Strings;

public static class PowerStrings
{
    public const string Ant = "Equal to the number of ants on your side of the board (currently {0})";
    public const string Lammergeier = "Equal to half the number of bones you have (currently {0} bones, meaning {1} damage)";
    public const string SacrificesThisTurn = "Equal to the number of cards you've sacrificed this turn (currently {0})";
    public const string Mirror = "Equal to the power of the card opposite this one";
    public const string MirrorInPlay = $"{Mirror} (currently {{0}})";
    public const string BellProximity = "Ranges from 4 to 1, depending on which lane you place it in. Lane 0 means 4 attack, lane 1 means 3 attack, lane 2 means 2 attack and lane 3 means 1 attack.";
    public const string BellProximityInPlay = $"{BellProximity} (currently {{0}})";
    public const string CardsInHand = "Equal to the number of cards in your hand (currently {0})";
}
