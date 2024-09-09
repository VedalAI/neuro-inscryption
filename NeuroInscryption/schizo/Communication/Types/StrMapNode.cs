namespace NeuroInscryption.Communication.Types;

internal sealed class StrMapNode : StrObject
{
    public string Name { get; }
    public string Description { get; }

    private StrMapNode(string name, string description)
    {
        Name = name;
        Description = description;
    }

    // Combat Events
    public static readonly StrMapNode CardBattle = new("Card Battle", "A standard card battle");
    public static readonly StrMapNode TotemBattle = new("Totem Card Battle", "A more difficult battle where your opponent has a totem of their own");
    public static readonly StrMapNode BossBattle = new("Boss Battle", "A difficult battle against a boss");

    // Non-combat events
    public static readonly StrMapNode BuildTotem = new("The Woodcarver", $"Obtain a random totem piece and optionally rebuild your totem. If you already have 7 totem pieces, you will receive an ${StrCard.Amalgam} card instead");
    public static readonly StrMapNode BuyPelts = new("The Trapper", "Purchase pelts or the knife using the teeth you have accumulated");
    public static readonly StrMapNode TradePelts = new("The Trader", "")
    public static StrMapNode CardChoiceRandom => new("Random Card Choice", "Choose a new card from a random selection of 3 cards");
    public static StrMapNode CardChoiceCost => new("Random Card Choice", "Choose a new card from a random selection of 3 cards of a specified cost");
    public static StrMapNode CardChoiceTribe => new("Random Card Choice", "Choose a new card from a random selection of 3 cards of a specified tribe");

}
