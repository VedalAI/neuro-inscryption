using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using NeuroInscryption.Utilities;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
internal static class StoryEvents
{
    [HarmonyPatch(typeof(SanctumSceneSequencer), nameof(SanctumSceneSequencer.UseFilmRollSequence))]
    [HarmonyPostfix]
    private static void UseFilmRoll(object __instance)
    {
        if (__instance.GetEnumeratorState() == 9) Reactions.LeshyPhotoTaken.Send();
    }
}