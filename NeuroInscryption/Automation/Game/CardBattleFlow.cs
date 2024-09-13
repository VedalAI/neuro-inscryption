using System.Collections;
using System.Linq;
using System.Reflection;
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

public sealed class CardBattleFlow : AutomationFlow<object>
{
    public override MethodBase EnableHook => AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TurnManager), nameof(TurnManager.PlayerTurn)));

    public override bool AllowInterruption => true;

    protected override bool IsValid(object enumerator)
    {
        return enumerator.GetEnumeratorState() == 8;
    }

    protected override IEnumerator Execute(object enumerator)
    {
        GameState.Board state = GameState.Board.Calculate();

        ChooseCardAction chooseCardAction = new(state);

        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply(chooseCardAction);

        Decision decision = new()
        {
            Id = "card_battle_turn",
            State = state,
            Query = DecisionStrings.CardBattleDecision,
            ResponseSchema = responseSchema
        };

        Response<Action> response = decision.Send();
        yield return response;
        Action? action = response.Result;

        if (action == null)
        {
            Plugin.Logger.LogFatal("Received invalid action, something broke along the way, I am sorry but IDK what do to. Restarting flow I guess...");
            yield return Execute(enumerator);
            yield break;
        }

        if (action.Match(chooseCardAction, out string? card))
        {
            PlayableCard target = state.PlayerHand.Find(c => c.Name == card).TargetCard!;
            Plugin.Logger.LogWarning("Moving to card " + target);

            yield return NeuroMouse.MoveTo(target.Anim.cardRenderer);
            NeuroMouse.ClickOnce();
        }
    }
}
