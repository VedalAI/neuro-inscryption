using System;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
public static class CardEvents
{
    [HarmonyPatch(typeof(AnglerBattleSequencer), nameof(AnglerBattleSequencer.OnOtherCardDie))]
    [HarmonyPostfix]
    private static void SpawnShark(object __instance)
    {
        if (__instance.GetEnumeratorState() == 3) Reactions.BaitBucketDied.Send();
    }

    [HarmonyPatch(typeof(TalkingCard), nameof(TalkingCard.PlayLine), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void TalkingCardDialogue(object __instance)
    {
        if (__instance.GetEnumeratorState() is 2 or 3) Reactions.TalkingCardSpoke.Send();
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
                Reactions.TentacleCardSpotted.Send();
            }

            if (card.Info.SpecialAbilities.Any(a => a is SpecialTriggeredAbility.RandomCard))
            {
                Reactions.GlitchedCardSpotted.Send();
            }

            if (GetCustomDeathcard(card.Info) is { } mod)
            {
                Reactions.DeathCardSpotted.Send(mod.nameReplacement);
            }
        }
    }

    [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.OnPlayed))]
    [HarmonyPostfix]
    private static void OnCardPlayed(PlayableCard __instance)
    {
        if (__instance.OpponentCard && GetCustomDeathcard(__instance.Info) is { } mod)
        {
            Reactions.LeshyPlayedDeathCard.Send(mod.nameReplacement);
        }
    }

    private static CardModificationInfo? GetCustomDeathcard(CardInfo card)
    {
        return card.Mods.FirstOrDefault(m => m.deathCardInfo != null && m.nameReplacement is not ("Reginald" or "Kaycee" or "Louis" or "Kaminski"));
    }
}