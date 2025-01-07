using NeuroInscryption.Utilities.Strings;

namespace NeuroInscryption.Communication.Strings;

public static class CardStrings
{
    public const string FreeCost = "free";
    public const string CostPrefix = "cost of ";
    public static readonly FormatString BloodCost = "{0} blood";
    public static readonly FormatString BonesCost = "{0} {0:bone|bones}";
    public static readonly FormatString IjiraqBonesCost = BonesCost + " (but won't consume {0:it|them} when played)";
    public const string InfoSeparator = ", ";
    public static readonly FormatString Description = "[{0}, {1} power, {2} health{3}{4}, {5}]";
    public const string IjiraqNarrowedDownShortDescription = "maybe Ijiraq";
    public static readonly FormatString IjiraqNarrowedDown = "Careful! This card or another {0} card in your hand is a disguised Ijiraq!";
    public static readonly FormatString GenericDiscriminator = " ({0})";
    public static readonly FormatString OnBoardDiscriminator = " (Lane {0})";

    public const string IjiraqPresent = "Be careful, any card in your hand or deck could be a disguised Ijiraq.";
}
