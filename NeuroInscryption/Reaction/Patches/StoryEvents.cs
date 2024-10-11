using DiskCardGame;
using GBC;
using HarmonyLib;
using NeuroInscryption.Reaction.Strings;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class StoryEvents
{
    [HarmonyPatch(typeof(VideoCameraRig), nameof(VideoCameraRig.EnterFootageMenu))]
    [HarmonyPostfix]
    private static void EnterFootageMenu()
    {
        Reactions.VideoBrowsingStarted.Send();
    }

    [HarmonyPatch(typeof(ChooseEyeballSequencer), nameof(ChooseEyeballSequencer.EyeballSelectedSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void WizardEyeballSelected(object __instance)
    {
        if (__instance.GetEnumeratorState() == 8 && RunState.Run.eyeState == EyeballState.Wizard) Reactions.WizardEyeballSelected.Send();
    }

    [HarmonyPatch(typeof(SanctumSceneSequencer), nameof(SanctumSceneSequencer.UseFilmRollSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void UseFilmRoll(object __instance)
    {
        if (__instance.GetEnumeratorState() == 9) Reactions.LeshyPhotoTaken.Send();
    }

    [HarmonyPatch(typeof(GBCIntroScene), nameof(GBCIntroScene.IntroSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void Act2Started(object __instance)
    {
        if (__instance.GetEnumeratorState() == 3) Reactions.Act2Started.Send();
    }
}