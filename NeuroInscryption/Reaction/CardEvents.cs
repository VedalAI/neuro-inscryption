using System;
using System.Linq;
using System.Runtime.InteropServices;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using NeuroInscryption.Utilities;
using UnityEngine;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
public static class CardEvents
{
    private static readonly ReactionTracker _baitBucketReaction = new("A dangerous Great White shark appears from within the recently destroyed bucket! That was unexpected!", Priority.Context, true);

    private static readonly ReactionTracker _talkingCardReaction = new("One of the cards begins to speak, it seems to be sentient!", Priority.Context, true);

    private static readonly ReactionTracker _tentacleCardReaction = new("One of the cards is very weird looking, it has tentacles all over it and its attack doesn't make any sense.", Priority.Context, true);

    private static readonly ReactionTracker _glitchedCardReaction = new("One of the cards appears to be broken and is glitching out, is that TV static?", Priority.Context, true);

    private static readonly ReactionTracker _deathcardReaction = new("That's {0}, the card you named! Can't believe we get to use it!", Priority.Context, true);

    [HarmonyPatch(typeof(AnglerBattleSequencer), nameof(AnglerBattleSequencer.OnOtherCardDie))]
    [HarmonyPostfix]
    private static void SpawnShark(object __instance)
    {
        if (__instance.GetEnumeratorState() == 3) _baitBucketReaction.Send();
    }

    [HarmonyPatch(typeof(TalkingCard), nameof(TalkingCard.PlayLine), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void TalkingCardDialogue(object __instance)
    {
        if (__instance.GetEnumeratorState() is 2 or 3) _talkingCardReaction.Send();
    }

    [HarmonyPatch(typeof(SelectableCard), nameof(SelectableCard.Initialize), typeof(Texture), typeof(Action<SelectableCard>), typeof(Action<SelectableCard>), typeof(Action<SelectableCard>))]
    [HarmonyPatch(typeof(SelectableCard), nameof(SelectableCard.Initialize), typeof(CardInfo), typeof(Action<SelectableCard>), typeof(Action<SelectableCard>), typeof(bool), typeof(Action<SelectableCard>))]
    private static void CardRevealedHandler(SelectableCard __instance)
    {
        if (!__instance.Info) return;

        if (__instance.Flipped)
        {
            __instance.cardFlipped += sendReaction;
        }
        else
        {
            sendReaction(__instance);
        }

        static void sendReaction(SelectableCard card)
        {
            if (card.Info.SpecialAbilities.Any(a => a is SpecialTriggeredAbility.BellProximity or SpecialTriggeredAbility.Mirror or SpecialTriggeredAbility.CardsInHand))
            {
                _tentacleCardReaction.Send();
            }

            if (card.Info.SpecialAbilities.Any(a => a is SpecialTriggeredAbility.RandomCard))
            {
                _glitchedCardReaction.Send();
            }

            if (card.Info.Mods.FirstOrDefault(m => m.deathCardInfo != null && m.nameReplacement is not ("Reginald" or "Kaycee" or "Louis" or "Kaminski")) is { } mod)
            {
                _deathcardReaction.Send(mod.nameReplacement);
            }
        }
    }
}