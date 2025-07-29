#if DEBUG && FALSE
using System.Linq;
using DiskCardGame;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

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
        __instance.deckIcons[0].Info.cards =
        [
            CardLoader.GetCardByName("PeltHare"),
            CardLoader.GetCardByName("PeltHare"),
            CardLoader.GetCardByName("PeltHare"),
            CardLoader.GetCardByName("PeltHare"),
            CardLoader.GetCardByName("PeltWolf"),
            CardLoader.GetCardByName("PeltWolf"),
            CardLoader.GetCardByName("PeltWolf"),
            CardLoader.GetCardByName("PeltGolden"),
            CardLoader.GetCardByName("PeltGolden"),
        ];
    }
}
#endif
