using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class PlaceCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSlot))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(1)
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        CombatState state = CombatState.Calculate();
        PlayableCard card = PlayerHand.Instance.ChoosingSlotCard;

        // todo: this is an IAction<int>, replace just like Neuro.Loadout.Choose but with combat context
        PlaceCardAction placeCardAction = new(state.PlayerLanes);

        Description decisionString = DecisionStrings.PlaceCard.Format(state.PlayerHand.First(c => c.TargetCard == card).GetDescription());
        Decision decision = new(decisionString, state);
        decision.AddAction(placeCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(placeCardAction, out int lane)) action.ThrowInvalid();

        CardSlot slot = BoardManager.Instance.playerSlots[lane];
        await NeuroMouse.MoveAndClick(slot, ct, msDelayBeforeClick: 500);
    }
}
