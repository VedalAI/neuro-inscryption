using System.Linq;
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

    [HarmonyPatch(typeof(AscensionUnlockSchedule), nameof(AscensionUnlockSchedule.NumStarterDecksUnlocked))]
    [HarmonyPostfix]
    private static void Postfix(ref int __result)
    {
        __result = AscensionUnlockSchedule.instance.unlockTiers.Count(t => t.starterDeckUnlocked != null);
    }

    [HarmonyPatch(typeof(AscensionChooseStarterDeckScreen), nameof(AscensionChooseStarterDeckScreen.OnEnable))]
    [HarmonyPostfix]
    private static void Postfix(AscensionChooseStarterDeckScreen __instance)
    {
        __instance.deckIcons[0].Info.cards = [CardLoader.GetCardByName("Ijiraq"), CardLoader.GetCardByName("Ijiraq"), CardLoader.GetCardByName("Ijiraq"), CardLoader.GetCardByName("Geck"), CardLoader.GetCardByName("Geck"), CardLoader.GetCardByName("Stoat"), CardLoader.GetCardByName("Stoat")];
    }
}
#endif
