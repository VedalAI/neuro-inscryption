using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Communication.Data.Helpers;

[HarmonyPatch]
public static class WoodcarverTotemTracker
{
    public static TotemData? LastTotem { get; private set; }

    [HarmonyPatch(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.BuildTotem), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void TrackTotem(IEnumerator __instance)
    {
        if (__instance.GetState() != 0) return;

        if (Part1ItemsManager.Instance.Totems.Count > 0)
        {
            LastTotem = TotemData.Create(Part1ItemsManager.Instance.Totems[0]);
        }
    }
}
