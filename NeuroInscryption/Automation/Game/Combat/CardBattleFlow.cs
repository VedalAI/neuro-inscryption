using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class CardBattleFlow : AutomationFlow<object>
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
            IsValid = context => context.GetResult<bool>() == false,
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        if (enumerator.GetType().Name.Contains(nameof(PlayerHand.SelectSlotForCard)))
        {
            await UniTask.Delay(1000, cancellationToken: ct); // time it takes for the card to go back in the hand
        }

        ViewManager.Instance.SwitchToView(BoardManager.Instance.DefaultView);

        CombatState state = CombatState.Calculate();

        PlayCardAction playCardAction = new(state);
        AcceptSurrenderAction acceptSurrenderAction = new(state);
        EndTurnAction endTurnAction = new();

        Decision decision = new(DecisionIds.CardBattleTurn, state, DecisionStrings.CardBattle);
        decision.AddAction(playCardAction);
        decision.AddAction(acceptSurrenderAction);
        decision.AddAction(endTurnAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(playCardAction, out PlayableCardData? cardData))
        {
            await NeuroMouse.MoveToAsync(cardData.TargetCard, 500, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else if (action.Match(acceptSurrenderAction))
        {
            await NeuroMouse.MoveToAsync(LeshyAnimationController.Instance.oliveBranchInteractable, 500, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else if (action.Match(endTurnAction))
        {
            await NeuroMouse.MoveToAsync(((BoardManager3D) BoardManager.Instance).Bell, 500, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
