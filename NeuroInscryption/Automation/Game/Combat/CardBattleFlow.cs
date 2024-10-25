using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Input.Helpers;
using NeuroInscryption.Utilities.Enumerators;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;
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
            IsValid = context => context.GetResult<bool>() == false
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(ConsumableItemSlot), nameof(ConsumableItemSlot.ConsumeItem))),
            IsValid = context => context.GetResult<bool>() == false
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        if (!enumerator.GetType().Name.Contains(nameof(TurnManager.PlayerTurn)))
        {
            await UniTask.Delay(500, cancellationToken: ct); // time it takes for the card to go back in the hand
        }

        await UniTask.Delay(500, cancellationToken: ct);

        ViewManager.Instance.SwitchToView(BoardManager.Instance.DefaultView);

        CombatState state = CombatState.Calculate();

        ChooseAction<PlayableCardData> playCardAction = new(state.PlayerHand.Where(c => c.IsPlayable == true), ActionStrings.PlayCard);
        ChooseAction<ItemData> useItemAction = new(state.Items, ActionStrings.UseItem);
        AcceptSurrenderAction acceptSurrenderAction = new(state.IsOpponentSurrendering);
        EndTurnAction endTurnAction = new();

        Decision decision = new(DecisionStrings.CardBattleTurn, state);
        decision.AddAction(playCardAction);
        decision.AddAction(useItemAction);
        decision.AddAction(acceptSurrenderAction);
        decision.AddAction(endTurnAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(playCardAction, out PlayableCardData? cardData))
        {
            await NeuroMouse.MoveToAsync(cardData.TargetCard, 500, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else if (action.Match(useItemAction, out ItemData? itemData))
        {
            await NeuroMouse.MoveToAsync(itemData.Slot!, 500, ct);
            await NeuroMouse.ClickAsync(ct);
            await InputHelpers.WaitUntilViewUnlocked(ct);
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
