using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.Strings;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class StoryEvents
{
    [HarmonyPatch(typeof(VideoCameraRig), nameof(VideoCameraRig.EnterFootageMenu))]
    [HarmonyPostfix]
    private static void EnterFootageMenu()
    {
        Reactions.VideoBrowsingStarted.Send();
    }
}