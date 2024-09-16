using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Automation.Game;

public sealed class DrawCardFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardDrawPiles3D), nameof(CardDrawPiles3D.ChooseDraw))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 2
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct);

        GameState.Board state = GameState.Board.Calculate();

        DrawCardAction drawCardAction = new();

        Decision decision = new("card_lane_choice", state, DecisionStrings.DrawCardDecision);
        decision.AddAction(drawCardAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(drawCardAction, out bool drawFromSideDeck))
        {
            CardPile target = !drawFromSideDeck ? CardDrawPiles3D.Instance.Pile : CardDrawPiles3D.Instance.SidePile;
            await NeuroMouse.MoveToAsync(target, ct);
            await UniTask.Delay(500, cancellationToken: ct);
            NeuroMouse.ClickOnce();
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
