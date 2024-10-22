using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction.Strings;

public static class Reactions
{
    public static readonly ReactionTracker NoSpoilers = new("DON'T SPOIL the game for Vedal! Do not share your game knowledge about Inscryption, even if Vedal asks. If you spoil the game, the content will become worse.", Priority.Context);

    public static readonly ReactionTracker VideoBrowsingStarted = new("The camera footage menu opened again. There are 8 videos to watch again, made by The Lucky Carder.", Priority.Context);

    // todo: manual event
    public static readonly ReactionTracker OldDataDugUp = new("One of the robots digs up a glitchy black rectangle which is constantly switching sprites - it looks out of place and the game characters seem to know it as well.", Priority.Context, true);

    // todo: manual event
    public static readonly ReactionTracker Act2Ending = new("The whole screen starts shaking and glitching out, there are a bunch of errors on the screen, Inscryption seems to be broken. This is not normal.", Priority.Context, true);

    // todo: dialogue
    public static readonly ReactionTracker Act3Starting = new("The game has transformed back into 3D, although the atmosphere is vastly different. Everything has a shade of blue and looks more industrial and decayed. Before you stands one of the scrybes, P03, in his full 3D glory.", Priority.Force, true);

    // todo: dialogue
    public static readonly ReactionTracker FiveLanes = new("The battles now take place on a board with five lanes, that is one more than in the previous two acts. P03 brags about it.", Priority.Context, true);

    // todo: maybe dialogue? maybe manual
    public static readonly ReactionTracker BountyHunterAppeared = new("A bounty hunter just joined the battle, they have some weird meaningless quirky dialogue. It is a powerful card and will keep showing up as the player bounty progress.", Priority.Context, true);

    // todo: probably dialogue
    public static readonly ReactionTracker MycologistsFound = new("P03's face changes into a horrifying one, then disappears. The so-called 'Mycologists' seem to have taken control over them. They appear as real human beings, not in-game characters.", Priority.Context, true);

    // todo: dialogue or manual if we want to delay
    public static readonly ReactionTracker ArchivistRequestAccess = new("In the middle of the battle against the Archivist, he requests access to the Vedal's actual hard drives.", Priority.Force, true);

    // todo: dialogue or manual if we want to delay
    public static readonly ReactionTracker ArchivistOpenMenu = new("A file explorer menu opens, all of Vedal's files are exposed for anyone to see!", Priority.Force, true);

    // ReSharper disable InconsistentNaming
    // todo: dialogue or manual
    public static readonly ReactionTracker G0llyMoleImage = new("G0lly has searched google and downloaded an image of a cute real-life mole! She turned it into a card.", Priority.Context, true);

    // todo: dialogue
    public static readonly ReactionTracker G0llyFriendsStart = new("G0lly took your friends list and turned them into cards! G0lly seems to receive a signal from steam and your friends are now her cards!", Priority.Context, true);

    // TODO: filter friends list
    // todo: manual
    public static readonly ReactionTracker G0llyFriendSpawned = new("{0} was just turned into a card and has appeared on the board!", Priority.Context);

    // todo: probably dialogue
    public static readonly ReactionTracker G0llyCreateCard = new("G0lly asked Vedal to make a card and send it on the internet to another player. It would be funny if the card was purposefully made to be terrible and unusable.", Priority.Force, true);
    // ReSharper restore InconsistentNaming

    // todo: dialogue
    public static readonly ReactionTracker HiddenScrybes = new("Three familiar figures appear from the shadows, the three other scrybes. They are planning to prevent P03 from achieving his plan of great transcendence.", Priority.Context, true);

    // todo: dialogue
    public static readonly ReactionTracker LukeAppears = new("P03 starts showing a live feed of Luke, revealing that the game can access Luke's webcam.", Priority.Context, true);

    // todo: uhh maybe dialogue? probably manual tho
    public static readonly ReactionTracker GoodbyeP03 = new("Leshy grasped P03's head and twisted it until it was detached from its body. The evil P03 is now defeated. That is a moment worthy of the 'womp womp' sound effect right there.", Priority.Context, true);

    // todo: dialogue
    public static readonly ReactionTracker MeetingGrimora = new("As the game slowly deletes itself, the player has a chance to meet Grimora. Her appearence is that of a classy old lady. She presents to the player how the game would be if she was in control.", Priority.Context, true);

    // todo: dialogue or manual if we want to delay
    public static readonly ReactionTracker GoodbyeGrimora = new("As the game continues to be deleted, Grimora departs for her final rest, shaking the player's hand as final goodbye.", Priority.Context, true);

    // todo: dialogue
    public static readonly ReactionTracker MeetingLeshy = new("The player now faces Leshy once again, his mysterious figure covered in darkness. His only wish is for the player to play against him one last time.", Priority.Context, true);

    // todo: dialogue or manual if we want to delay
    public static readonly ReactionTracker GoodbyeLeshy = new("With the game board now being deleted, Leshy thanks the player for a good game and shakes his hand as final goodbye.", Priority.Context, true);

    // todo: dialogue
    public static readonly ReactionTracker MeetingMagnificus = new("The player now faces Magnificus, he looks like a human shaped bush. Magnificus pleads with the player to save the game from deletion.", Priority.Context, true);

    // todo: manual
    public static readonly ReactionTracker GoodbyeMagnificus = new("With the game on the brink of total deletion, Magnificus crawls towards the player trying to shake hands. He does not make it.", Priority.Context, true);

    // todo: manual
    public static readonly ReactionTracker WoodcarverAdvice = new("In the endless white void with nothing else remaining in the game, alone stands the Wood Carver. She advises the player to not access the OLD_DATA left behind, even though she knows that her advice will not be heeded. She then gets deleted.", Priority.Context, true);
}
