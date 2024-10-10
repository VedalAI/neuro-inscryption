using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using NeuroInscryption.Utilities;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
internal static class StoryEvents
{
    private static readonly ReactionTracker _leshyPhotoReaction = new("The mysterious old man is now trapped in a card! We can escape now.", Priority.Context, true);

    [HarmonyPatch(typeof(SanctumSceneSequencer), nameof(SanctumSceneSequencer.UseFilmRollSequence))]
    [HarmonyPostfix]
    private static void UseFilmRoll(object __instance)
    {
        if (__instance.GetEnumeratorState() == 9) _leshyPhotoReaction.Send();
    }
}