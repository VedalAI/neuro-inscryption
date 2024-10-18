using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction.Strings;

public static class Reactions
{
    public static readonly ReactionTracker NoSpoilers = new("DON'T SPOIL the game for Vedal! Do not share your game knowledge about Inscryption, even if Vedal asks. If you spoil the game, the content will become worse.", Priority.Context);

    public static readonly ReactionTracker VideoBrowsingStarted = new("The camera footage menu opened again. There are 8 videos to watch again, made by The Lucky Carder.", Priority.Context);

    public static readonly ReactionTracker OldDataDugUp = new("One of the robots digs up a glitchy black rectangle which is constantly switching sprites - it looks out of place and the game characters seem to know it as well.", Priority.Context, true);

    public static readonly ReactionTracker Act2Ending = new("The whole screen starts shaking and glitching out, there are a bunch of errors on the screen, Inscryption seems to be broken. This is not normal.", Priority.Context, true);

    public static readonly ReactionTracker Act3Starting = new("The game has transformed back into 3D, although the atmosphere is vastly different. Everything has a shade of blue and looks more industrial and decayed. Before you stands one of the scrybes, P03, in his full 3D glory.", Priority.Force, true);

    public static readonly ReactionTracker FiveLanes = new("", Priority.Context, true);

    public static readonly ReactionTracker BridgeThreeLanes = new("", Priority.Context, true);

    public static readonly ReactionTracker BountyHunterAppeared = new("", Priority.Context, true);

    public static readonly ReactionTracker MycologistsFound = new("", Priority.Force, true);

    public static readonly ReactionTracker ArchivistRequestAccess = new("", Priority.Context, true);

    public static readonly ReactionTracker ArchivistOpenMenu = new("", Priority.Context, true);

    public static readonly ReactionTracker UnfinishedFaceCreated = new("", Priority.Context, true);

    // ReSharper disable InconsistentNaming
    public static readonly ReactionTracker G0llyPullsImage = new("", Priority.Context, true);

    public static readonly ReactionTracker G0llyPullsSteamFriend = new("", Priority.Context, true);

    public static readonly ReactionTracker G0llyCreateCard = new("", Priority.Context, true);
    // ReSharper restore InconsistentNaming

    public static readonly ReactionTracker HiddenScrybes = new("", Priority.Context, true);

    public static readonly ReactionTracker LukeAppears = new("", Priority.Context, true);

    // play womp womp
    public static readonly ReactionTracker GoodbyeP03 = new("", Priority.Context, true);

    public static readonly ReactionTracker MeetingGrimora = new("", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeGrimora = new("", Priority.Context, true);

    public static readonly ReactionTracker MeetingMagnificus = new("", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeMagnificus = new("", Priority.Context, true);

    public static readonly ReactionTracker MeetingLeshy = new("", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeLeshy = new("", Priority.Context, true);

    public static readonly ReactionTracker WoodcarverAdvice = new("", Priority.Context, true);
}