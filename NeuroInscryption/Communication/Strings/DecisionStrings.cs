using NeuroInscryption.Communication.Data.Describable;

namespace NeuroInscryption.Communication.Strings;

public static class DecisionStrings
{
    public static readonly Description DrawCard = ("draw_card_choice", "Choose if you want to draw a guaranteed Squirrel card, or a random card from your main deck.");
    public static readonly Description CardBattleTurn = ("card_battle_turn", "Choose the next action to take during this card battle.");
    public static readonly DescriptionFormat ChooseSacrifices = ("card_sacrifice_choice", "You chose to play {0}. Choose which cards to sacrifice for it.");
    public static readonly DescriptionFormat PlaceCard = ("card_lane_choice", "You chose to play {0}. Choose which lane to place it in.");
    public static readonly Description MagpieDrawCard = ("specific_draw_card_choice", "Choose any card to draw from your deck.");

    public static readonly Description ChoosePath = ("path_choice", "Choose a path to continue on. Take into account where each path leads to next.");
    public static readonly Description ChooseCard = ("card_choice", "Choose a card to add to your deck.");
    public static readonly Description CampfireChooseCard = ("campfire_card_choice", "Choose a card to upgrade at the campfire from the available options. Take into account the type of upgrade it is and the current state of your deck.");
    public static readonly Description CampfireRiskUpgrade = ("campfire_risk_second_upgrade", "You can upgrade the card again, but there is a 50% chance that the card will be destroyed and removed from your deck. If your card dies, you will get a Hoggy Bank item [use to immediately receive 4 bones] if your pack is not full.");
    public static readonly Description CampfireNoRiskUpgrade = ("campfire_second_upgrade", "You can upgrade the card again, with no risk.");
    public const string CampfireAttackBoost = "+1 Power";
    public const string CampfireHealthBoost = "+2 Health";
    public static readonly Description Sacrifice = ("sacrifice_choice", "Choose a card to sacrifice and remove from your deck. You will be able to move the sigils from the chosen card to another card in your deck that has not already been empowered. Please take into account the current state of your deck and what card you want to empower next.");
    public static readonly DescriptionFormat SacrificeEmpower = ("empower_choice", "You chose to sacrifice {0}. Choose an unempowered host card to receive the sigils from the sacrificed card.");
    public static readonly Description ChooseTotemPiece = ("totem_piece_choice", "Choose a totem piece to add to your collection. A full totem consists of a Sigil piece and a Tribe piece and grants the chosen Sigil to all cards belonging to the chosen Tribe.");
    public static readonly Description BuildTotem = ("build_totem", "You have enough totem pieces to build a totem. Choose a tribe to apply the sigil to.");
    public static readonly Description BoneAltarChooseCard = ("sacrifice_card_choice", "Choose any card to sacrifice to the Bone Lord, removing it from your deck permanently in exchange for a Minor Boon of the Bone Lord [start each battle with 1 extra bone]. Sacrificing a Black Goat will instead give a Boon of the Bone Lord [start each battle with 8 extra bones]. If you sacrifice a Pelt, you will receive nothing.");
    public static readonly Description GoobertChooseCard = ("copy_card_choice", "Choose a card to copy from the available options. You will receive a duplicate of that card which might have an imperfection, such as a modified sigil, or an increased/decreased attack/health.");
    public static readonly Description MycologistsChoosePair = ("merge_duplicates_choice", "Choose a pair of duplicate cards to combine them together into a stronger version with the sum of the original cards' stats. Sigils will be combined as well.");
    public static readonly Description ChooseBoulder = ("random_boulder_choice", "Choose a random boulder to break. 2 of the boulders contain an insect card with a random additional sigil, and 1 boulder contains a Golden Pelt.");
    public static readonly Description ChooseDeckTrial = ("deck_trial_choice", "Choose a deck trial. Afterwards, 3 random cards will be selected from your deck to check if they meet the requirements of the trial. If you pass, you will get to choose one of three cards with 2 additional sigils each. If you fail, you will not get anything.");
    public static readonly Description ChooseItem = ("item_choice", "Choose an item to add to your pack.");
    public static readonly DescriptionFormat ChooseTraderCards = ("trade_card_choice", "Choose {0} {0:card|cards} to buy from the Trader and add to your deck");
    public static readonly DescriptionFormat ChooseTraderCardsBattle = ("trade_card_choice", "Choose {0} {0:card|cards} to buy from the Trader and add to your hand. The rest of the cards will fight on the Trader's side.");
    public static readonly DescriptionFormat BuyTrapperItems = ("trapper_buy_choice", "You have {0} {0:tooth|teeth} to spend. Choose what items to buy from the Trapper. You can also leave if you do not want to buy anything.");
}
