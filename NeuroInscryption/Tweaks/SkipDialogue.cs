using DiskCardGame;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
public static class SkipDialogue
{
    [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ManagedUpdate))]
    [HarmonyPostfix]
    private static void ManagedUpdate(TextDisplayer __instance)
    {
        __instance.continuePressed = true;
    }
}
