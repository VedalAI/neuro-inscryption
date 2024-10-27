using DiskCardGame;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
internal static class SkipDialogue
{
    [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ManagedUpdate))]
    [HarmonyPostfix]
    private static void AutoContinue(TextDisplayer __instance)
    {
        __instance.continuePressed = true;
    }

    [HarmonyPatch(typeof(ProgressionData), nameof(ProgressionData.IntroducedCard))]
    [HarmonyPostfix]
    public static void DisableCardTutorials(ref bool __result)
    {
        __result = true;
    }
}
