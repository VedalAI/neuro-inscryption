using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using NeuroInscryption.Utilities;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
public static class CardEvents
{
    private static readonly ReactionTracker _baitBucketReaction = new(num => num switch
    {
        0 => ("A dangerous Great White shark appears from within the recently destroyed bucket! That was unexpected!", Priority.Context),
        _ => null
    });

    private static readonly ReactionTracker _talkingCardReaction = new(num => num switch
    {
        0 => ("One of the cards begins to speak, it seems to be sentient!", Priority.Context),
        _ => null
    });

    [HarmonyPatch(typeof(AnglerBattleSequencer), nameof(AnglerBattleSequencer.OnOtherCardDie))]
    [HarmonyPostfix]
    private static void SpawnShark(object enumerator)
    {
        if (enumerator.GetEnumeratorState() == 3) _baitBucketReaction.Send();
    }

    [HarmonyPatch(typeof(TalkingCard), nameof(TalkingCard.PlayLine), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void TalkingCardDialogue(object enumerator)
    {
        if (enumerator.GetEnumeratorState() == 2) _talkingCardReaction.Send();
    }
}