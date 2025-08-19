using System.Collections;
using System.Text;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Utilities.Enumerators;
using NeuroSdk.Messages.Outgoing;
using UnityEngine;

namespace NeuroInscryption.Communication.ContextMessages;

[HarmonyPatch]
file static class Attacks
{
    private static readonly StringBuilder _contextBuilder = new();
    private static CardSlot? _lastSlot;

    [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.PlayerTurn), MethodType.Enumerator)]
    [HarmonyPrefix]
    public static void CapturePlayerTurn(IEnumerator __instance)
    {
        if (__instance.GetState() != 10) return;

        _contextBuilder.Clear();
        _contextBuilder.Append("Combat initiated. ");
    }
    
    [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.OpponentTurn), MethodType.Enumerator)]
    [HarmonyPrefix]
    public static void CaptureOpponentTurn(IEnumerator __instance, bool __result)
    {
        if (__instance.GetState() != 2) return;
        
        _contextBuilder.Append("It is your opponent's turn. ");
    }
    
    [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.OpponentTurn), MethodType.Enumerator)]
    [HarmonyPostfix]
    public static void SendContext(IEnumerator __instance)
    {
        if (__instance.GetState() is not (6 or 7)) return;
        
        Context.Send(_contextBuilder.ToString(), true);
    }

    [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.QueuedSlot), MethodType.Setter)]
    [HarmonyPrefix]
    public static void CapturePlayingSlot(PlayableCard __instance, [HarmonyArgument(0)] CardSlot value)
    {
        if (__instance.QueuedSlot != null && value == null)
        {
            _lastSlot = __instance.QueuedSlot;
        }
    }
    
    [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.OnPlayedFromOpponentQueue))]
    [HarmonyPrefix]
    public static void CaptureCardPlayed(PlayableCard __instance)
    {
        if (_lastSlot == null)
        {
            Debug.LogError("Last slot not assigned when card captured, this should never happen");
            _contextBuilder.Append($"Their {__instance.Info.DisplayedNameEnglish} was played in a slot. ");
        }
        else
        {
            _contextBuilder.Append($"Their {__instance.Info.DisplayedNameEnglish} was played in slot {_lastSlot.Index + CombatState.LaneIndexBase}. ");
        }
    }

    [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.AttackIsBlocked))]
    [HarmonyPostfix]
    public static void CaptureAttackBlock(PlayableCard __instance, CardSlot opposingSlot, bool __result)
    {
        if (__result)
        {
            _contextBuilder.Append($"{P(__instance)} {__instance.Info.DisplayedNameEnglish} could not attack {Pl(opposingSlot.Card)} {opposingSlot.Card.Info.DisplayedNameEnglish}! ");
        }
    }
    
    [HarmonyPatch(typeof(CombatPhaseManager3D), nameof(CombatPhaseManager3D.VisualizeCardAttackingDirectly), MethodType.Enumerator)]
    [HarmonyPrefix]
    public static void CaptureOpponentDamage(IEnumerator __instance)
    {
        CardSlot attackingSlot = __instance.GetFieldValue<CardSlot>("attackingSlot");
        CardSlot targetSlot = __instance.GetFieldValue<CardSlot>("targetSlot");
        int damage = __instance.GetFieldValue<int>("damage");
        
        _contextBuilder.Append($"{P(attackingSlot.Card)} {attackingSlot.Card.Info.DisplayedNameEnglish} attacked {D(targetSlot)} directly for {damage} damage. ");
    }
    
    [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.TakeDamage))]
    [HarmonyPrefix]
    public static void CaptureAttack(PlayableCard __instance, int damage, PlayableCard attacker)
    {
        _contextBuilder.Append($"{P(attacker)} {attacker.Info.DisplayedNameEnglish} dealt {damage} damage to {Pl(__instance)} {__instance.Info.DisplayedNameEnglish}. ");
    }

    private static string P(PlayableCard card) => card.OpponentCard ? "Their" : "Your";
    private static string Pl(PlayableCard card) => P(card).ToLowerInvariant();
    private static string D(CardSlot slot) => !slot.IsPlayerSlot ? "the opponent" : "you";
}