using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using NeuroInscryption.Utilities;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
internal static class ItemEvents
{
    private static readonly ReactionTracker _pliersReaction = new(num => num switch
    {
        0 => ("The player used some pliers to pluck their own tooth out!!!! What a gruesome display and such disturbing sounds!", Priority.React),
        1 or 2 => ("The player used some pliers to pluck their own tooth out again! It's as gruesome and disturbing as ever!", Priority.Context),
        _ => null
    });

    private static readonly ReactionTracker _knifeReaction = new(num => num switch
    {
        0 => ("The player used a dagger to carve one of their OWN EYES out!!!!!!!!!!!", Priority.React),
        1 or 2 => ("The player used a dagger to carve one of their own eyes out again!", Priority.Context),
        _ => null,
    });

    [HarmonyPatch(typeof(PliersItem), nameof(PliersItem.ActivateSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void Pliers(object enumerator)
    {
        if (enumerator.GetEnumeratorState() == 6) _pliersReaction.Send();
    }

    [HarmonyPatch(typeof(SpecialDaggerItem), nameof(SpecialDaggerItem.ActivateSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void Knife(object enumerator)
    {
        if (enumerator.GetEnumeratorState() == 4) _knifeReaction.Send();
    }
}