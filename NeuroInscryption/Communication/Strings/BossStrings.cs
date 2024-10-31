namespace NeuroInscryption.Communication.Strings;

public static class BossStrings
{
    // TODO: Related sigils and traits (and also check this for other strings)

    public const string Prospector = "You are currently in a boss battle against the Prospector.";
    public const string ProspectorPhase1Ability = "If you kill his Pack Mule, you will get 4 extra cards.";
    public const string ProspectorPhase1Transition = "After defeating his first phase, he will kill all cards on your side of the board and replace them with Gold Nuggets [0 power, 2 health, cannot be sacrificed], as well as summon a Bloodhound [2 power, 3 health, Guardian].";

    public const string Angler = "You are currently in a boss battle against the Angler.";
    public const string AnglerPhase1Ability = "Every other turn, he will aim his hook at one of your cards in order to steal it. He will always target the newest card played, so playing new cards will divert his attention.";
    public const string AnglerPhase1TransitionEasy = "After defeating his first phase, he will clear his side of the board and summon Bait Buckets [0 power, 1 health; on death, gets replaced with Great White [4 power, 2 health, Waterborne]] in front of all of your cards.";
    public const string AnglerPhase1TransitionHard = "After defeating his first phase, he will clear his side of the board and summon Great Whites [4 power, 2 health, Waterborne] in front of all of your cards.";
    public const string AnglerHookTarget = "This card is being targeted by the Angler's hook. If the target is not changed, this card will be hooked by the Angler at the end of his next turn, and he will steal it. Opponent cards in this lane will be dragged back.";

    public const string Trapper = "You are currently in a boss battle against the Trapper.";
    public const string Trader = "You are currently in a boss battle against the Trader.";
    public const string TrapperAndTraderPhase1Transition = "After defeating his first phase, he will transform into the Trader, who will then fill her side of the board with cards. She will put all Pelts on your side of the board back into your hand and give you one extra Pelt. Then, you can exchange each Pelt in your hand for one of her cards, making it fight for you instead.";

    public const string Leshy = "You are currently in a boss battle against Leshy.";
    public const string LeshyPhases1And2Ability = "At the start of his turn, he will alternate between doing one of two things: (1) Put on a mask representing The Prospector, The Angler, or The Trader, in the following order: Prospector -> Angler -> Trader -> repeat. (2) If Leshy already has a mask on, he will perform a special action and then remove the mask: 1. The Prospector: Kills all cards on your side and replaces them with Gold Nuggets (0 power, 1 health). 2. The Angler: Steals your most recently played card and drags it to his side. 3. The Trader: Plays two cards with an extra Sigil, gives you one Wolf Pelt, and allows you to trade the pelt to take one card from the opponent's side into your hand.";
    public const string LeshyPhase1Transition = "After defeating his first phase, he will put Grand Firs (0 power, 3 health, Mighty Leap) in front of Airborne cards, and Stumps (0 power, 3 health) in front of others. Cards with Bifurcated Strike are ignored.";
    public const string LeshyPhase2Transition = "After defeating his second phase, he will clear his side of the board and play The Moon (1 power, 40 health, Mighty Leap, Omni Strike, Tidal Lock, Made of Stone). After The Moon is eliminated, he will not play any more cards.";

    public const string Royal = "You are currently in a boss battle against Royal Dominguez.";
    public const string RoyalPhase1And2Ability = "He will aim his cannons at the board which will deal 10 damage each to two random spaces.";
    public const string RoyalPhase1Transition = "After defeating his first phase, he will give you a card pack containing 4 random cards from the following list: Squirrel, Rabbit, Mole, Mole Man, Beaver, Opposum, Field Mice, Pack Rat and Stoat.";
    public const string RoyalPhase2Transition = "After defeating his second phase, he will clear his side of the board and play The Limoncello (2 power, 80 health, Mighty Leap, Omni Strike, Made of Stone). After The Limoncello is eliminated, he will not play any more cards.";
    public const string RoyalCannonTargetCard = "This space is being targeted by Royal's cannons. This card will take 10 damage at the end of his next turn.";
    public const string RoyalCannonTargetEmpty = "This space is being targeted by Royal's cannons. If any card moves to it, it will take 10 damage at the end of his next turn.";
}
