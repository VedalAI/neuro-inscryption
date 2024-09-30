namespace NeuroInscryption.Communication.Strings;

public static class BossStrings
{
    public const string Prospector = "You are currently in a boss battle against the Prospector.";
    public const string ProspectorPhase1Ability = "If you kill his Pack Mule, you will get 4 extra cards.";
    public const string ProspectorPhase1Transition = "After defeating his first phase, he will kill all cards on your side of the board and transform them into Gold Nuggets (0 power, 2 health, cannot be sacrificed), as well as summon a Bloodhound (2 power, 3 health, Guardian).";

    public const string Angler = "You are currently in a boss battle against the Angler.";
    public const string AnglerPhase1Ability = "Every other turn, he will aim his hook at one of your cards in order to steal it. He will always target the newest card played, so you can play new cards to divert his attention.";
    public const string AnglerPhase1TransitionEasy = "After defeating his first phase, he will clear his side of the board and summon Bait Buckets (0 power, 1 health, will summon Great White [4 power, 2 health, Waterborne] when destroyed) in front of all of your cards.";
    public const string AnglerPhase1TransitionHard = "After defeating his first phase, he will clear his side of the board and summon Great Whites (4 power, 2 health, Waterborne) in front of all of your cards.";
    public const string AnglerHookTarget = "This card will be hooked by the Angler at the end of his next turn, and he will steal it. Opponent cards opposing this one will be dragged back.";

    public const string TrapperAndTraderPhase1 = "You are currently in a boss battle against the Trapper.";
    public const string TrapperAndTraderPhase1Transition = "After defeating his first phase, he will transform into the Trader and fill all 8 of her spaces with cards. She will put all played Pelts (which are still alive) back in your hand and give you one extra Pelt. Each Pelt that you have in your hand can be used to take one of her cards, but any not traded for will fight for her.";
    public const string TrapperAndTraderPhase2 = "You are currently in a boss battle against the Trader.";

    public const string Leshy = "You are currently in a boss battle against Leshy.";
    public const string LeshyPhases1And2Ability = "At the start of his turn, he will alternate between doing one of two things: (1) Put on a mask representing The Prospector, The Angler, or The Trader. The Prospector will always be first, followed by The Angler, then finally The Trader, before looping back to The Prospector. (2) If Leshy already has a mask on, he will perform a special action and then remove the mask: 1. The Prospector: He will destroy all cards on your side and replace them with Gold Nuggets (0 power, 1 health). 2. The Angler: He will steal the most recently played card and move it to his side. 3. The Trader: He will play two cards with an extra Sigil, give you one Wolf Pelt, and allow you to trade the pelt for one card on his side.";
    public const string LeshyPhase1Transition = "After defeating his first phase, he will put Grand Firs (0 power, 3 health, Mighty Leap) in front of Airborne cards, and Stumps (0 power, 3 health) in front of others. Cards with Bifurcated Strike are ignored.";
    public const string LeshyPhase2Transition = "After defeating his second phase, he will then play The Moon (1 power, 40 health, Mighty Leap, Omni Strike, Tidal Lock, Made of Stone), replacing all of his current cards. After The Moon is eliminated, no more cards will be played by him.";

    public const string Royal = "You are currently in a boss battle against Royal Dominguez.";
    public const string RoyalPhase1And2Ability = "He will aim his cannons at the board which will deal 10 damage each to two random spaces.";
    public const string RoyalPhase1Transition = "After defeating his first phase, he will give you a card pack containing 4 random cards from the following list: Squirrel, Rabbit, Mole, Mole Man, Beaver, Opposum, Field Mice, Pack Rat and Stoat.";
    public const string RoyalPhase2Transition = "After defeating his second phase, he will then play The Limoncello (2 power, 80 health, Mighty Leap, Omni Strike, Made of Stone), replacing all of his current cards. After The Limoncello is eliminated, no more cards will be played by him.";
    public const string RoyalCannonTargetCard = "This space this card is in is being targeted by Royal's cannons. It will take 10 damage at the end of his next turn.";
    public const string RoyalCannonTargetEmpty = "This space is being targeted by Royal's cannons. If any card moves to it, it will take 10 damage at the end of his next turn.";
}
