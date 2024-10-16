namespace NeuroInscryption.Communication.Strings;

public static class PowerStrings
{
    public const string Ant = "Equal to the number of Ants on your side of the board";
    public const string Lammergeier = "Equal to half the number of bones you have";
    public const string SacrificesThisTurn = "Equal to the number of cards you've sacrificed this turn";
    public const string Mirror = "Equal to the power of the card opposite this one";
    public const string BellProximity = "Power ranges from 1 to 4 based on proximity to the bell, which is left of lane 0. Additionally, gains 1 power for every adjacent Chime.";
    public const string CardsInHand = "Equal to the number of cards in your hand";

    public const string InPlaySuffix = " (currently {0})";
    public const string LammergeierInPlaySuffix = " (currently {0} bones, meaning {1} damage)";
}
