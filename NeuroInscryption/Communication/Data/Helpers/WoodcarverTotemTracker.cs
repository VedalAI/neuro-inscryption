using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Data.Helpers;

[HarmonyPatch]
public static class WoodcarverTotemTracker
{
    public static TotemData? LastTotem { get; private set; }

    [HarmonyPatch(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.BuildTotem), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void TrackTotem(object __instance)
    {
        if (__instance.GetEnumeratorState() != 0) return;

        if (Part1ItemsManager.Instance.Totems.Count > 0)
        {
            LastTotem = TotemData.Create(Part1ItemsManager.Instance.Totems[0]);
        }
    }
}
