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

        ChooseClickAction<PlayableCardData> playCardAction = new(state.PlayerHand.Where(c => c.IsPlayable == true), c => c.TargetCard, 500, ActionStrings.PlayCard);
        ChooseClickAction<ItemData> useItemAction = new(state.Items.Where(i => i.IsUsable == true), i => i.Slot!, 500, ActionStrings.UseItem);
        AcceptSurrenderAction acceptSurrenderAction = new(state.IsOpponentSurrendering);
        EndTurnAction endTurnAction = new();

        AsyncActionWindow window = new(DecisionStrings.CardBattleTurn, state);
        window.AddAction(playCardAction);
        window.AddAction(useItemAction);
        window.AddAction(acceptSurrenderAction);
        window.AddAction(endTurnAction);
        await window.SendAsync(ct);
    }
}
