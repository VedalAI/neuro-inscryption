using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Communication.Websocket;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
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
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 1
        }
    ];

    protected override IEnumerator Execute(object enumerator)
    {
        GameState.Board state = GameState.Board.Calculate();
        PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

        ChooseSacrificesAction chooseSacrificesAction = new(card, state);

        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply(chooseSacrificesAction);

        Decision decision = new()
        {
            Id = "card_sacrifice_choice",
            State = state,
            Query = string.Format(DecisionStrings.ChooseSacrificesDecision, state.PlayerHand.Find(c => c.TargetCard == card).GetDescription()),
            ResponseSchema = responseSchema
        };

        Response<Action> response = decision.Send();
        yield return response;
        Action? action = response.Result;

        if (action == null)
        {
            Plugin.Logger.LogError("Received invalid action, something broke along the way, I am sorry but IDK what do to. Restarting flow I guess...");
            yield return Execute(enumerator);
            yield break;
        }

        if (action.Match(chooseSacrificesAction, out IEnumerable<string>? sacrifices))
        {
            IEnumerable<PlayableCard> targets = sacrifices.Select(sacrifice => state.PlayerHand.Find(c => c.Name == sacrifice).TargetCard!);

            foreach (PlayableCard target in targets)
            {
                yield return NeuroMouse.MoveTo(target);
                yield return new WaitForSeconds(0.5f);
                NeuroMouse.ClickOnce();
                yield return new WaitForSeconds(0.25f);
            }
        }
        else
        {
            Plugin.Logger.LogError("Received action which did not match possible expected actions. Flow will be restarted.");
            yield return Execute(enumerator);
        }
    }
}
