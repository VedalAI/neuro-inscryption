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
        if (message.ToLowerInvariant() == "wrapitup")
        {
            Reactions.PlayerTouchedEverything.Send();
            Plugin.Logger.LogMessage($"Twitch Backdoor - Sending WrapItUp reaction");
        }
        else if (message.ToLowerInvariant() == "blindge")
        {
            Reactions.PlayerCantReadUI.Send();
            Plugin.Logger.LogMessage($"Twitch Backdoor - Sending Blindge reaction");
        }
    }
}