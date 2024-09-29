using NeuroInscryption.Communication.Data.Describable;

namespace NeuroInscryption.Communication.Strings;

public class TrapperStrings
{
    public const string TeethCost = "{0} {0:tooth|teeth}";

    public static readonly Description RabbitPelt = ("Rabbit Pelt", "A pelt card with 1 health and 0 power. Can be traded at the Trader for a regular card.");
    public static readonly Description WolfPelt = ("Wolf Pelt", "A pelt card with 2 health and 0 power. Can be traded at the Trader for a regular card with 1 additional sigil.");
    public static readonly Description GoldenPelt = ("Golden Pelt", "A pelt card with 3 health and 0 power. Can be traded at the Trader for a rare card.");
    public static readonly Description SkinningKnife = ("Skinning Knife", "A consumable item that allows you to destroy one of your opponent's cards during a card battle. You will also gain a pelt in your hand after. You can only purchase this item once per run.");
}
