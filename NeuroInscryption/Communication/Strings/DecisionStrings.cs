using NeuroInscryption.Communication.Data.Describable;

namespace NeuroInscryption.Communication.Strings;

public static class DecisionStrings
{
    public static readonly Description DrawCard = ("draw_card_choice", "Choose if you want to draw a guaranteed Squirrel card, or a random card from your main deck.");
    public static readonly Description CardBattleTurn = ("card_battle_turn", "Choose the next action to take during this card battle.");
    public static readonly DescriptionFormat ChooseSacrifices = ("card_sacrifice_choice", "You have chosen to play {0}. Choose which cards to sacrifice for it.");
    public static readonly DescriptionFormat PlaceCard = ("card_lane_choice", "You have chosen to play {0}. Choose which lane to place it in.");

    public static readonly Description ChoosePath = ("path_choice", "Choose a path to continue on. Take into account where each path leads to next.");
    public static readonly Description ChooseCard = ("card_choice", "Choose a card to add to your deck from the provided list of options.");
    public static readonly Description CampfireChooseCard = ("campfire_card_choice", "Choose a card to upgrade at the campfire. Take into account the type of upgrade it is and the current state of your deck.");
    public static readonly Description CampfireRiskUpgrade = ("campfire_risk_second_upgrade", "You can upgrade the card again, but there is a 50% chance of failure. Your card might be gone forever. Are you sure you want to take the risk? If your card dies, you will get a Hoggy Bank item [use to immediately receive 4 bones] if your pack is not full.");
    public static readonly Description CampfireNoRiskUpgrade = ("campfire_second_upgrade", "You can upgrade the card again. It would probably be wise to do so.");
    public const string CampfireAttackBoost = "+1 Power";
    public const string CampfireHealthBoost = "+2 Health";
    public static readonly Description ChooseTotemPiece = ("totem_piece_choice", "Choose a totem piece to add to your collection. In order to create a full totem, you need a Sigil piece and a Tribe piece. All cards belonging to that tribe will then receive the sigil.");
    public static readonly Description BuildTotem = ("build_totem", "You have all the pieces needed to build a totem. Choose a tribe to apply the sigil to.");
    public static readonly Description BoneAltarChooseCard = ("sacrifice_card_choice", "Choose a card to sacrifice to the Bone Lord, removing it from your deck permanently. You will get a Minor Boon of the Bone Lord in exchange, allowing you to start each battle with one extra bone. If you choose to sacrifice a Black Goat instead, you will receive a Boon of the Bone Lord, allowing you to start each battle with 8 extra bones. If you sacrifice a Pelt, you will receive nothing.");
}
