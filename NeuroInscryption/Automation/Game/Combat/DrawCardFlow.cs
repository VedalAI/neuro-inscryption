using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class DrawCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardDrawPiles3D), nameof(CardDrawPiles3D.ChooseDraw))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(2)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct);

        CombatState state = CombatState.Calculate();

        DrawCardAction drawCardAction = new();

        Decision decision = new(DecisionStrings.DrawCard, state);
        decision.AddAction(drawCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(drawCardAction, out bool drawFromSideDeck)) action.ThrowInvalid();

        CardPile target = !drawFromSideDeck ? CardDrawPiles3D.Instance.Pile : CardDrawPiles3D.Instance.SidePile;
        await NeuroMouse.MoveToAsync(target, 500, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
