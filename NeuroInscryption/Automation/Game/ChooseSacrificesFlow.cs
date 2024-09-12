using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Json.Actions;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Automation.Game;

public sealed class ChooseSacrificesFlow : AutomationFlow<object>
{
    public override MethodBase EnableHook => AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSacrificesForCard)));

    protected override bool IsValid(object target)
    {
        return target.GetEnumeratorState() == 1;
    }

    protected override IEnumerator Execute(object enumerator)
    {
        PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply(new ChooseSacrificesAction(card));

        GameState.Board state = GameState.Board.Current;
        CardData cardData = state.PlayerHand.Find(c => c.TargetCard == card.Info);

        Decision decision = new()
        {
            Id = "card_sacrifice_choice",
            State = state,
            Query = $"You have chosen to play {cardData.GetDescription()}. Choose which cards to sacrifice for it.",
            ResponseSchema = responseSchema
        };

        GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(decision, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        // TODO: Send decision
        // TODO: Get back sacrifices
        // TODO: Click cards

        yield break;
    }
}
