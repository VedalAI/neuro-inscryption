using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game;

public sealed class PlaceCardFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSlot))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 1
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        CombatState state = CombatState.Calculate();
        PlayableCard card = PlayerHand.Instance.ChoosingSlotCard;

        PlaceCardAction placeCardAction = new(state);

        string query = string.Format(DecisionStrings.PlaceCardDecision, state.PlayerHand.Find(c => c.TargetCard == card).GetDescription());
        Decision decision = new("card_lane_choice", state, query);
        decision.AddAction(placeCardAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(placeCardAction, out int lane))
        {
            CardSlot slot = BoardManager.Instance.playerSlots[lane];
            await NeuroMouse.MoveToAsync(slot, 500, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
