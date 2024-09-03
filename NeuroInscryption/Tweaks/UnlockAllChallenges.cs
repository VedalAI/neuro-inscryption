using DiskCardGame;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
internal static class UnlockAllChallenges
{
    [HarmonyPatch(typeof(AscensionUnlockSchedule), nameof(AscensionUnlockSchedule.ChallengeIsUnlockedForLevel))]
    [HarmonyPostfix]
    private static void Postfix(ref bool __result)
    {
        __result = true;
    }
}
