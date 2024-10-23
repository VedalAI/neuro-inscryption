using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction.Strings;

public static class Reactions
{
    public static readonly ReactionTracker NoSpoilers = new("DON'T SPOIL the game for Vedal! Do not share your game knowledge about Inscryption, even if Vedal asks. If you spoil the game, the content will become worse.", Priority.Context);

    public static readonly ReactionTracker VideoBrowsingStarted = new("The camera footage menu opened again. There are 8 videos to watch again, made by The Lucky Carder.", Priority.Context);

    public static readonly ReactionTracker OldDataDugUp = new("One of the robots digs up a glitchy black rectangle which is constantly switching sprites - it looks out of place and the game characters seem to know it as well.", Priority.Context, true);

    public static readonly ReactionTracker Act2Ending = new("The whole screen starts shaking and glitching out, there are a bunch of errors on the screen, Inscryption seems to be broken. This is not normal.", Priority.Context, true);

    public static readonly ReactionTracker Act3Starting = new("The game has transformed back into 3D, although the atmosphere is vastly different. Everything has a shade of blue and looks more industrial and decayed. Before you stands one of the Scrybes, P03, in his full 3D glory.", Priority.Force, true);

    public static readonly ReactionTracker FiveLanes = new("The battles now take place on a board with five lanes, that is one more than in the previous two acts. P03 brags about it.", Priority.Context, true);

    public static readonly ReactionTracker MycologistsFound = new("P03's face changes into a horrifying one, then disappears. The so-called 'Mycologists' seem to have taken control over him. They appear as real human beings, not in-game characters.", Priority.Context, true);

    public static readonly ReactionTracker ArchivistPickFile = new("Vedal has given The Archivist access to the following file: {0}", Priority.Context, true);

    // ReSharper disable InconsistentNaming
    public static readonly ReactionTracker G0llyMoleImage = new("G0lly has searched Google and downloaded an image of a cute real-life mole! She turned it into a card.", Priority.Context, true);

    public static readonly ReactionTracker G0llyFriendsStart = new("G0lly has accessed Vedal's friends list on Steam and is creating cards with the friends' names and avatars!", Priority.Context, true);

    public static readonly ReactionTracker G0llyFriendSpawned = new("{0} was just turned into a card and has appeared on the board!", Priority.Context);

    public static readonly ReactionTracker G0llyCreateCard = new("G0lly asked Vedal to make a card and send it to another player on the Internet. It would be funny if the card was purposefully made to be terrible and unusable.", Priority.Force, true);
    // ReSharper restore InconsistentNaming

    public static readonly ReactionTracker HiddenScrybes = new("Three familiar figures appear from the shadows; it's the three other Scrybes. They are planning to stop P03 from achieving his plan, the Great Transcendence.", Priority.Context, true);

    public static readonly ReactionTracker LukeAppears = new("P03's face briefly changes to show a live feed of Luke's webcam as he addresses Luke directly. After a moment of shock, Luke hastily puts his palm over the webcam's iris.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeP03 = new("Leshy grasped P03's head and twisted it until it was detached from its body. The evil bot is dead. That is a moment worthy of the 'womp womp' sound effect right there.", Priority.Force, true);

    public static readonly ReactionTracker MeetingGrimora = new("As the game slowly deletes itself, the player has a chance to meet Grimora. Her appearance is that of a classy old lady. She presents to the player how the game would be if she was in control.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeGrimora = new("As the game continues to be deleted, Grimora departs for her final rest, shaking the player's hand as a final goodbye.", Priority.Context, true);

    public static readonly ReactionTracker MeetingLeshy = new("The player now faces Leshy once again, his mysterious figure covered in darkness. His only wish is for the player to play against him one last time.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeLeshy = new("The game board has been deleted. Leshy thanks the player for a good game and offers a handshake as a final goodbye.", Priority.Context, true);

    public static readonly ReactionTracker MeetingMagnificus = new("The player now faces Magnificus, whose 3D form looks like a human-shaped bush. Magnificus pleads with the player to save the game from deletion.", Priority.Context, true);

    public static readonly ReactionTracker GoodbyeMagnificus = new("With the game on the brink of total deletion, Magnificus crawls towards the player trying to shake hands. He does not make it.", Priority.Context, true);
}
