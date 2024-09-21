using NeuroInscryption.Communication.Data;

namespace NeuroInscryption.Communication.Strings;

public static class DecisionStrings
{
    public static readonly Describable DrawCard = ("draw_card_choice", "Choose if you want to draw a guaranteed Squirrel card, or a random card from your main deck.");
    public static readonly Describable CardBattleTurn = ("card_battle_turn", "Choose the next action to take during this card battle.");
    public static readonly DescribableFormat ChooseSacrifices = ("card_sacrifice_choice", "You have chosen to play {0}. Choose which cards to sacrifice for it.");
    public static readonly DescribableFormat PlaceCard = ("card_lane_choice", "You have chosen to play {0}. Choose which lane to place it in.");

    public static readonly Describable ChoosePath = ("path_choice", "Choose a path to continue on. Take into account where each path leads to next.");
    public static readonly Describable ChooseCard = ("card_choice", "Choose a card to add to your deck from the provided list of options.");
    public static readonly Describable CampfireChooseCard = ("campfire_card_choice", "Choose a card to upgrade at the campfire. Take into account the type of upgrade it is and the current state of your deck.");
    public static readonly Describable CampfireRiskUpgrade = ("campfire_risk_second_upgrade", "You can upgrade the card again, but there is a 50% chance of failure. Your card might be gone forever. Are you sure you want to take the risk? If your card dies, you will get a Hoggy Bank item [use to immediately receive 4 bones] if your pack is not full.");
    public static readonly Describable CampfireNoRiskUpgrade = ("campfire_second_upgrade", "You can upgrade the card again. It would probably be wise to do so.");
    public const string CampfireAttackBoost = "+1 Power";
    public const string CampfireHealthBoost = "+2 Health";
}
