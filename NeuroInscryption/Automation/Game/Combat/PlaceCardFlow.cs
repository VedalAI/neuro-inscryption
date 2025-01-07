using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

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

        PlaceCardAction placeCardAction = new(BoardManager.Instance.playerSlots);
        string decisionQuery = DecisionStrings.PlaceCard.Format(state.PlayerHand.First(c => c.TargetCard == card).GetDescription());

        int lane = await Neuro.Common.Choose(placeCardAction, decisionQuery, state, ct);

        CardSlot slot = BoardManager.Instance.playerSlots[lane];
        await NeuroMouse.MoveAndClick(slot, ct, msDelayBeforeClick: 500);
    }
}
