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
    public static readonly Description CampfireChooseCard = ("campfire_card_choice", "Choose a card to upgrade at the campfire from the available options. Take into account the type of upgrade it is and the current state of your deck.");
    public static readonly Description CampfireRiskUpgrade = ("campfire_risk_second_upgrade", "You can upgrade the card again, but there is a 50% chance of failure. Your card might be gone forever. Are you sure you want to take the risk? If your card dies, you will get a Hoggy Bank item [use to immediately receive 4 bones] if your pack is not full.");
    public static readonly Description CampfireNoRiskUpgrade = ("campfire_second_upgrade", "You can upgrade the card again. It would probably be wise to do so.");
    public const string CampfireAttackBoost = "+1 Power";
    public const string CampfireHealthBoost = "+2 Health";
    public static readonly Description ChooseTotemPiece = ("totem_piece_choice", "Choose a totem piece to add to your collection. In order to create a full totem, you need a Sigil piece and a Tribe piece. All cards belonging to that tribe will then receive the sigil.");
    public static readonly Description BuildTotem = ("build_totem", "You have all the pieces needed to build a totem. Choose a tribe to apply the sigil to.");
    public static readonly Description BoneAltarChooseCard = ("sacrifice_card_choice", "Choose any card to sacrifice to the Bone Lord, removing it from your deck permanently. You will get a Minor Boon of the Bone Lord in exchange, allowing you to start each battle with one extra bone. If you choose to sacrifice a Black Goat instead, you will receive a Boon of the Bone Lord, allowing you to start each battle with 8 extra bones. If you sacrifice a Pelt, you will receive nothing.");
    public static readonly Description GoobertChooseCard = ("copy_card_choice", "Choose a card to copy from the available options. You will receive a duplicate of that card which might have an imperfection, such as a modified sigil, or an increased/decreased attack/health.");
    public static readonly Description MycologistsChoosePair = ("merge_duplicates_choice", "Choose an identical card pair to combine the two versions of the card together and create a stronger version with stats equal to the combined stats of the original cards. Sigils will be combined as well, but duplicate sigils will only be counted once");
    public static readonly Description ChooseBoulder = ("random_boulder_choice", "Choose a random boulder to break. 2 of the boulders contain an insect card with a random additional sigil, and 1 boulder contains a Golden Pelt.");
    public static readonly Description ChooseDeckTrial = ("deck_trial_choice", "Choose a deck trial from the available options. Afterwards, 3 random cards will be selected from your deck to check if they meet the requirements of the trial. If you pass, you will get a choice of 3 random cards with 2 additional sigils eaceh. If you fail, you will not get anything.");
}
