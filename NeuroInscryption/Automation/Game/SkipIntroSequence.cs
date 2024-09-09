using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game;

[HarmonyPatch]
internal static class SkipIntroSequence
{
    [HarmonyPatch(typeof(RunIntroSequencer), nameof(RunIntroSequencer.RunIntroSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void RunIntroSequence(object __instance)
    {
        ref int state = ref __instance.GetStateField();

        if (state == 18)
        {
            ViewManager.Instance.SwitchToView(View.MapDefault);
        }
    }
}
