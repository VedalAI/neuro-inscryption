using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction.Strings;

public static class Reactions
{
    public static readonly ReactionTracker NoSpoilers = new("DON'T SPOIL the game for Vedal! Do not share your game knowledge about Inscryption, even if Vedal asks. If you spoil the game, the content will become worse.", Priority.Context);

    // Cards

    public static readonly ReactionTracker BaitBucketDied = new("A dangerous Great White shark suddenly appears from within the recently destroyed bucket!", Priority.Context, true);

    public static readonly ReactionTracker TentacleCardSpotted = new("One of the cards is very weird looking, it has tentacles all over it and its attack doesn't make any sense.", Priority.Context, true);

    public static readonly ReactionTracker GlitchedCardSpotted = new("One of the cards appears to be broken and is glitching out, is that TV static?", Priority.Context, true);

    public static readonly ReactionTracker DeathCardSpotted = new("That's {0}, the card you created! Can't believe we get to use it!", Priority.Context, true);

    public static readonly ReactionTracker LeshyPlayedDeathCard = new("The old man just played {0}, the card you created! It's now on the enemy's side.", Priority.Context, true);

    // Dialogue

    public static readonly ReactionTracker Campfire = new("The survivors of the campfire offer shelter to a creature. They claim that the chosen creature will gain a buff as a result - however, these survivors look famished and untrustworthy.", Priority.Context, true);

    public static readonly ReactionTracker TrapperAppeared = new("You took in the familiar sights and scents of The Trapper's pelts. However, the once friendly man now beholds you with a steely gaze. The third boss encounter is starting.", Priority.Context, true);

    public static readonly ReactionTracker TrapperSwappedToTrader = new("The Trapper flips their mask and becomes The Trader, revealing that they are the same person.", Priority.Context, true);

    public static readonly ReactionTracker LeshyRevealed = new("The mysterious figure finally appears visible in the light, he looks like an old man with leathery skin covered in leafage.", Priority.Context, true);

    public static readonly ReactionTracker LeshyPlayedMoon = new("The old man snapped a picture of the MOON ITSELF, capturing it within a massive, board-spanning card. This seems to be the last obstacle to defeat him.", Priority.Context, true);

    public static readonly ReactionTracker LeshyCake = new("The reward for defeating the old man is a disgusting pile of meat with a lit candle on top.", Priority.Context, true);

    public static readonly ReactionTracker LeshyCommentedOnPreviousRun = new("The mysterious figure refers to the last challenger winning against him. However, the cycle doesn't seem to be broken.", Priority.Context, true);

    public static readonly ReactionTracker GooBottleAppeared = new("A bottle filled with weird green goo appeared. There is a pair of eyes sticking out of the goo. It shakes and makes strange noises as it looks at you. Vedal should probably pick it up.", Priority.Force, true);

    // Gameplay

    public static readonly ReactionTracker PlayerLostBattle = new("Vedal just lost a card battle, he is now down to {0}.", Priority.Context);

    public static readonly ReactionTracker PlayerLostRun = new("Vedal just lost the run, what a loser, he now has to start all over again.", Priority.Force);

    // Items

    public static readonly ReactionTracker PliersUsed = new(num => num switch
    {
        0 => ("The player used some pliers to pluck their own tooth out!!!! What a gruesome display and such disturbing sounds!", Priority.Context),
        1 or 2 => ("The player used some pliers to pluck their own tooth out again! It's as gruesome and disturbing as ever!", Priority.Context),
        _ => null
    });

    public static readonly ReactionTracker DaggerUsed = new(num => num switch
    {
        0 => ("The player used a dagger to carve one of their OWN EYES out!!!!!!!!!!!", Priority.Context),
        1 or 2 => ("The player used a dagger to carve one of their own eyes out again!", Priority.Context),
        _ => null,
    });

    // Story

    public static readonly ReactionTracker VideoBrowsingStarted = new("A weird effect happens, the game appears to be a recording and now there is a camera menu, with videos.", Priority.Context, true);

    public static readonly ReactionTracker WizardEyeballSelected = new("Mysterious writings appear before the player: 'FIND SALVATION IN CUCKOO CLOCK'.", Priority.Context, true);

    public static readonly ReactionTracker LeshyPhotoTaken = new("The mysterious old man is now trapped in a card! We can escape now.", Priority.Context, true);

    public static readonly ReactionTracker Act2Started = new("After starting a new game, the game entirely changed and suddenly looks very different, everything is now pixelated and 2D. It's like a whole new game.", Priority.Context, true);
}