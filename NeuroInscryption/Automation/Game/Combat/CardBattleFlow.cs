using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;
using ItemData = NeuroInscryption.Communication.Data.Serializable.ItemData;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class CardBattleFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TurnManager), nameof(TurnManager.PlayerTurn))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(8),
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(PlayerHand), nameof(PlayerHand.SelectSlotForCard))),
            IsValid = FlowHookHelpers.EnumeratorFinished(),
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(ConsumableItemSlot), nameof(ConsumableItemSlot.ConsumeItem))),
            IsValid = FlowHookHelpers.EnumeratorFinished(),
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TargetSlotItem), nameof(TargetSlotItem.ActivateSequence))),
            IsValid = FlowHookHelpers.EnumeratorFinished(),
        },
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        if (!enumerator.GetType().Name.Contains(nameof(TurnManager.PlayerTurn)))
        {
            await UniTask.Delay(500, cancellationToken: ct); // time it takes for the card to go back in the hand
        }

        await Wait.CommonFlowDelay(ct);
        await Wait.ViewUnlocked(ct);

        ViewManager.Instance.SwitchToView(BoardManager.Instance.DefaultView);

        CombatState state = CombatState.Calculate();

        ChooseAction<PlayableCardData> playCardAction = new(state.PlayerHand.Where(c => c.IsPlayable == true), ActionStrings.PlayCard);
        ChooseAction<ItemData> useItemAction = new(state.Items.Where(i => i.IsUsable == true), ActionStrings.UseItem);
        AcceptSurrenderAction acceptSurrenderAction = new(state.IsOpponentSurrendering);
        EndTurnAction endTurnAction = new();

        Decision decision = new(DecisionStrings.CardBattleTurn, state);
        decision.AddAction(playCardAction);
        decision.AddAction(useItemAction);
        decision.AddAction(acceptSurrenderAction);
        decision.AddAction(endTurnAction);

        ActionBase pickedAction = await decision.Send(ct);

        InteractableBase? target = null;
        if (pickedAction == playCardAction)
        {
            target = playCardAction.GetResult().TargetCard;
        }
        else if (pickedAction == useItemAction)
        {
            target = useItemAction.GetResult().Slot!;
        }
        else if (pickedAction == acceptSurrenderAction)
        {
            await UniTask.Delay(500, cancellationToken: ct); // branch takes time to extend, so the mouse aim gets thrown off
            target = LeshyAnimationController.Instance.oliveBranchInteractable;
        }
        else if (pickedAction == endTurnAction)
        {
            target = ((BoardManager3D)BoardManager.Instance).Bell;
        }
        AutomationManager.Assert(target != null, "Did not pick valid target");
        await NeuroMouse.MoveAndClick(target, ct, msDelayBeforeClick: 500);
    }
}
