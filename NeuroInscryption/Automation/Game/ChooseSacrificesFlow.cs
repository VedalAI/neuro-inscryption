using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Automation.Game;

public sealed class ChooseSacrificesFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSacrificesForCard))),
            IsValid = context => context.GetInstance().GetEnumeratorState() == 1
        }
    ];

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(object enumerator)
    {
        GameState.Board state = GameState.Board.Calculate();
        PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply(new ChooseSacrificesAction(card, state));

        CardData cardData = state.PlayerHand.Find(c => c.TargetCard == card);

        Decision decision = new()
        {
            Id = "card_sacrifice_choice",
            State = state,
            Query = string.Format(DecisionStrings.ChooseSacrificesDecision, cardData.GetDescription()),
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
