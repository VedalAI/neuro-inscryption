using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class ItemEvents
{
    [HarmonyPatch(typeof(PliersItem), nameof(PliersItem.ActivateSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void Pliers(object __instance)
    {
        if (__instance.GetEnumeratorState() == 6) Reactions.PliersUsed.Send();
    }

    [HarmonyPatch(typeof(SpecialDaggerItem), nameof(SpecialDaggerItem.ActivateSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void Knife(object __instance)
    {
        if (__instance.GetEnumeratorState() == 4) Reactions.DaggerUsed.Send();
    }
}