using NeuroInscryption.Utilities.Strings;

namespace NeuroInscryption.Communication.Strings;

public static class ContextStrings
{
    public const string Rules = "You are now in a card battle. Your goal is to defeat your opponent by dealing enough damage to win. Your health is also your opponent's health, and is represented by a scale which starts at 0. Every point of damage you deal to the opponent adds 1 to the scale, and every point of damage the opponent deals to you removes 1 from the scale. You win if you deal enough damage to reach a scale value of 5. You lose if you take enough damage for the scale to reach -5. The game board is split into 4 lanes, facing each other. For example, your card in lane 1 faces the opponent's card in lane 1. When you end your turn, your cards attack in order from left to right (lane 1 to 4), then your opponent's queued cards move into their lanes if they are empty, then your opponent's cards attack from left to right. If a card attacks an opposing card, the opposing card will take damage equal to the card's attack. If there no opposing card in the lane, the opponent will instead take damage directly. The cards in your hand as well as the available blood on the board (amount of cards) are very important resources. You can only draw one card per turn, and you get to choose if you want to draw a Squirrel or draw from your main deck instead. If you have high cost cards but no way to play them, you should consider drawing a Squirrel. Because of this, you should sacrifice cards sparingly, only when it makes sense to do so to improve your position, otherwise you will just lose cards for no reason.";
    public static readonly FormatString CampfireBurned = "Your {0} was burned at the campfire! It has been removed from your deck";
    public const string DeckTrialFailed = "You failed the deck trial. You do not get a reward.";
    public static readonly FormatString AddCardToDeck = "You picked up a {0}.";
}
