using DiskCardGame;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

#if DEBUG
[HarmonyPatch]
internal static class UnlockAllChallengesAndDecks
{
    [HarmonyPatch(typeof(AscensionUnlockSchedule), nameof(AscensionUnlockSchedule.ChallengeIsUnlockedForLevel))]
    [HarmonyPatch(typeof(AscensionUnlockSchedule), nameof(AscensionUnlockSchedule.StarterDeckIsUnlockedForLevel))]
    [HarmonyPostfix]
    private static void Postfix(ref bool __result)
    {
        __result = true;
    }
}
#endif
