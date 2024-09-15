using System.Collections;
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

public class CardBattleFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TurnManager), nameof(TurnManager.PlayerTurn))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 8,
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(PlayerHand), nameof(PlayerHand.SelectSlotForCard))),
            IsValid = context => context.GetResult<bool>() == false && context.GetInstance()!.GetEnumeratorField<bool>("<cardWasPlayed>5__2") == false,
        }
    ];

    protected override IEnumerator Execute(object enumerator)
    {
        Plugin.Logger.LogWarning(enumerator.GetType().Name);
        if (enumerator.GetType().Name.Contains(nameof(PlayerHand.SelectSlotForCard)))
        {
            yield return new WaitForSeconds(1f); // time it takes for the card to go back in the hand
        }

        GameState.Board state = GameState.Board.Calculate();

        ChooseCardAction chooseCardAction = new(state);

        // TODO: Move this somewhere else so i dont need to write it every time
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

        // TODO: Move this somewhere else somehow so i dont need to copy paste it every time
        if (action == null)
        {
            Plugin.Logger.LogError("Received invalid action, something broke along the way, I am sorry but IDK what do to. Restarting flow I guess...");
            yield return Execute(enumerator);
            yield break;
        }

        if (action.Match(chooseCardAction, out string? card))
        {
            PlayableCard target = state.PlayerLanes.Values.First(c => c?.Name == card)!.TargetCard!;

            yield return NeuroMouse.MoveTo(target);
            yield return new WaitForSeconds(0.5f);
            NeuroMouse.ClickOnce();
        }
        else
        {
            Plugin.Logger.LogError("Received action which did not match possible expected actions. Flow will be restarted.");
            yield return Execute(enumerator);
        }
    }
}
