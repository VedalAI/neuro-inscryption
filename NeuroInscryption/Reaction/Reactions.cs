using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction;

public static class Reactions
{
    // Cards

    public static readonly ReactionTracker BaitBucketDied = new("A dangerous Great White shark appears from within the recently destroyed bucket! That was unexpected!", Priority.Context, true);

    public static readonly ReactionTracker TalkingCardSpoke = new("One of the cards begins to speak, it seems to be sentient!", Priority.Context, true);

    public static readonly ReactionTracker TentacleCardSpotted = new("One of the cards is very weird looking, it has tentacles all over it and its attack doesn't make any sense.", Priority.Context, true);

    public static readonly ReactionTracker GlitchedCardSpotted = new("One of the cards appears to be broken and is glitching out, is that TV static?", Priority.Context, true);

    public static readonly ReactionTracker DeathCardSpotted = new("That's {0}, the card you created! Can't believe we get to use it!", Priority.Context, true);

    public static readonly ReactionTracker LeshyPlayedDeathCard = new("The old man just played {0}, the card you created! It's now on the enemy's side.", Priority.Context, true);

    // Dialogue

    public static readonly ReactionTracker Campfire = new("The survivors of the campfire offer shelter to a creature. They claim that the chosen creature will gain a buff as a result, however, these survivors look famished and untrustworthy.", Priority.Context, true);

    public static readonly ReactionTracker ProspectorAppeared = new("A hobbled figure appeared... the first boss encounter: 'Heeeeeeee-haaw! 'Twas The Prospector!'. His mule seems very dear to him.", Priority.Context, true);

    public static readonly ReactionTracker ProspectorUsedAbility = new(num => num switch
    {
        0 => ("The Prospector just struck all played cards, killing them and replacing them with useless golden nuggets! He is breaking the rules of the game!", Priority.Context),
        1 => ("The Prospector replaced all cards with gold nuggets again.", Priority.Silent),
        _ => null
    });

    public static readonly ReactionTracker AnglerAppeared = new("A hulking man sat by a brackish pond with his feet submerged in the dark water. He pulled a hook from a pile of rotting fish and rose to his feet. The second boss encounter is the Angler. Go fish.", Priority.Context, true);

    public static readonly ReactionTracker TrapperAppeared = new("You took in the familiar sights and scents of The Trapper's pelts. However, the once friendly man now beheld you with a steelzy gaze. The third boss encounter is starting.", Priority.Context, true);

    public static readonly ReactionTracker TrapperSwappedToTrader = new("The Trapper flips their mask and becomes The Trader, revealing that they are the same person.", Priority.Context, true);

    public static readonly ReactionTracker GrizzlyWallSpawned = new(num => num switch
    {
        0 => ("The mysterious figure is a sore loser and is infuriated his boss encounters are getting defeated too fast. He resorts to cheating in order to prevent further progress! He plays an impassable wall of grizzlies.", Priority.Context),
        1 => ("The mysterious figure resorts to cheating once again and plays another impassable wall of grizzlies...", Priority.Context),
        >= 2 => ("Another wall of grizzlies is played by the cheating opponent.", Priority.Silent)
    });

    public static readonly ReactionTracker LeshyRevealed = new("The mysterious figure finally appears visible in the light, he looks like an old man with leathery skin covered in leafage.", Priority.Context, true);

    public static readonly ReactionTracker LeshyPlayedMoon = new("The old man snapped a picture of the MOON ITSELF, capturing it within a card. This seems to be the last obstacle to defeat him.", Priority.Context, true);

    public static readonly ReactionTracker Starvation = new("Having no cards to draw, starvation appears on the battlefield and occupies a card space. It is a race against time as each turn starvation will spread until no card spaces are left, and the player loses.", Priority.Context, true);

    public static readonly ReactionTracker LeshyCake = new("The reward for defeating the old man is a disgusting pile of meat with a lit candle on top.", Priority.Context, true);

    public static readonly ReactionTracker LeshyCommentedOnPreviousRun = new("The mysterious figure refers to the last challenger winning against him. However, the cycle doesn't seem to be broken.", Priority.Context, true);

    // Item

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

    public static readonly ReactionTracker LeshyPhotoTaken = new("The mysterious old man is now trapped in a card! We can escape now.", Priority.Context, true);
}