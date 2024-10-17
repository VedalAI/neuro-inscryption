using NeuroInscryption.Reaction.Strings;
using NeuroInscryption.Tweaks;

namespace NeuroInscryption.Reaction.Patches;

internal static class ManualEvents
{
    static ManualEvents()
    {
        TwitchIntegration.HandleTwitchMessage += HandleTwitchMessage;
    }

    internal static void HandleTwitchMessage(string message)
    {
        if (message.ToLowerInvariant() == "nospoilers")
        {
            Reactions.NoSpoilers.Send();
            Plugin.Logger.LogMessage($"Twitch Backdoor - Sending no spoilers reaction");
        }
    }
}