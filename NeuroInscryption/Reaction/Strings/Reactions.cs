using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction.Strings;

public static class Reactions
{
    public static readonly ReactionTracker NoSpoilers = new("DON'T SPOIL the game for Vedal! Do not share your game knowledge about Inscryption, even if Vedal asks. If you spoil the game, the content will become worse.", Priority.Context);

    public static readonly ReactionTracker VideoBrowsingStarted = new("The camera footage menu opened again. There are 8 videos to watch again, made by The Lucky Carder.", Priority.Context);

    public static readonly ReactionTracker OldDataDugUp = new("One of the robots digs up a glitchy black rectangle which is constantly switching sprites - it looks out of place and the game characters seem to know it as well.", Priority.Context, true);

    public static readonly ReactionTracker Act2Ending = new("The whole screen starts shaking and glitching out, there are a bunch of errors on the screen, Inscryption seems to be broken. This is not normal.", Priority.Context, true);

    public static readonly ReactionTracker Act3Starting = new("The game has transformed back into 3D, although the atmosphere is vastly different. Everything has a shade of blue and looks more industrial and decayed. Before you stands one of the scrybes, P03, in his full 3D glory.", Priority.Force, true);

    public static readonly ReactionTracker FiveLanes = new("The battles now take place on a board with five lanes, that is one more than in the previous two acts.", Priority.Context, true);

    public static readonly ReactionTracker BridgeThreeLanes = new("There is railings occupying the first and last lane, this board is now only three lanes, that is one less than in the previous two acts.", Priority.Context, true);

    public static readonly ReactionTracker BountyHunterAppeared = new("One of the bounty hunters just joined the battle, they are powerful cards and will keep showing up as the player bounty progress.", Priority.Context, true);

    public static readonly ReactionTracker MycologistsFound = new("P03 face changes into a horrifying one, it is the Mycologists and they claim to have taken control over P03.", Priority.Force, true);

    public static readonly ReactionTracker ArchivistRequestAccess = new("In the middle of the battle against the Archivist, he requires access to the player's hard drives.", Priority.Context, true);

    public static readonly ReactionTracker ArchivistOpenMenu = new("A menu opens and it appears to be file explorer, all files on the player's hard drives are accessible here.", Priority.Context, true);

    public static readonly ReactionTracker UnfinishedFaceCreated = new("P03 says that there is no boss here and you will have to create one, various faces are presented to you.", Priority.Context, true);

    // ReSharper disable InconsistentNaming
    public static readonly ReactionTracker G0llyPullsImage = new("G0lly is accessing google and looking up images of cute animals, even creating cards with said images.", Priority.Context, true);

    public static readonly ReactionTracker G0llyPullsSteamFriend = new("G0lly seems to receive a signal from steam and your friends are now her cards!", Priority.Context, true);

    public static readonly ReactionTracker G0llyCreateCard = new("G0lly requires you to make a card and send it on the internet so it reaches another player, you can make any type of card even funny or bad.", Priority.Context, true);
    // ReSharper restore InconsistentNaming

    public static readonly ReactionTracker HiddenScrybes = new("The player is meeting with familiar figures, who are the three other scrybes. They are planning together to prevent P03 from achieving his plan of great transcendence.", Priority.Context, true);

    public static readonly ReactionTracker LukeAppears = new("P03 face changes into the face of Luke, revealing that the game can access Luke's webcam.", Priority.Context, true);

    // play womp womp
    public static readonly ReactionTracker GoodbyeP03 = new("Leshy grasped P03 head and twisted it until it was detached from it's body. That is a womp womp moment right there.", Priority.Context, true);

    public static readonly ReactionTracker MeetingGrimora = new("The player now faces Grimora, she appears like a classy old lady, she presents to the player how the game would be if she was in control.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeGrimora = new("The game slowly deleting itself, Grimora departs for her final rest, shaking the player's hand as final goodbye.", Priority.Context, true);
  
    public static readonly ReactionTracker MeetingLeshy = new("The player now faces Leshy once again, his mysterious figure covered in darkness, he wishes for the player to play against him one last time.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeLeshy = new("The game board being deleted, Leshy thanks the player for a good game and shakes his hand as final goodbye.", Priority.Context, true);

    public static readonly ReactionTracker MeetingMagnificus = new("The player now faces Magnificus, he looks like a human shaped bush, Magnificus plead the player to save the game from deletion.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeMagnificus = new("The game on the brink of total deletion, Magnificus crawls towards the player trying to shake hands. He did not make it.", Priority.Context, true);

    public static readonly ReactionTracker WoodcarverAdvice = new("In the endless void with nothing else in the game stands the wood carver. She advised the player to not access the OLD_DATA left behind. She then gets deleted.", Priority.Context, true);
}
