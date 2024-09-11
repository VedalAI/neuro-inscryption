using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Json.Actions;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using GameState = NeuroInscryption.Communication.GameState;

namespace NeuroInscryption.Automation.Game;

// ReSharper disable once InconsistentNaming
internal sealed class FULL_CardBattleFlow : AutomationFlow
{
    public override MethodBase EnableHook => AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TurnManager), nameof(TurnManager.PlayerTurn)));

    public override bool IsValid(object enumerator)
    {
        if (Plugin.Config.SplitCardBattleDecisions) return false;

        int state = enumerator.GetEnumeratorState();
        return state == 8;
    }

    public override IEnumerator Execute(object enumerator)
    {
        TurnManager turnManager = enumerator.GetEnumeratorThis<TurnManager>();

        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply<FULL_PlayCardAction>();

        Decision decision = new()
        {
            Id = "card_battle_turn",
            State = GameState.Board.Current,
            Query = "Choose the next action to take during this card battle.",
            ResponseSchema = responseSchema
        };

        GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(decision, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        yield break;
    }
}
