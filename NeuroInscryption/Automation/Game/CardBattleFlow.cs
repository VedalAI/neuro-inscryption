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
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Automation.Game;

public sealed class CardBattleFlow : AutomationFlow<object>
{
    public override MethodBase EnableHook => AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TurnManager), nameof(TurnManager.PlayerTurn)));

    protected override bool IsValid(object enumerator)
    {
        return enumerator.GetEnumeratorState() == 8;
    }

    protected override IEnumerator Execute(object enumerator)
    {
        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply<ChooseCardAction>();

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

        // TODO: Send decision
        // TODO: Get back card name
        // TODO: Click card

        yield break;
    }
}
