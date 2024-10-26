using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class CampfireFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class CampfireBoostContext(bool isAttack)
    {
        public readonly string BoostType = isAttack ? DecisionStrings.CampfireAttackBoost : DecisionStrings.CampfireHealthBoost;
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardStatBoostSequencer), nameof(CardStatBoostSequencer.StatBoostSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(7),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        EnumeratorWrapper<CardStatBoostSequencer> wrapper = new(enumerator);
        CardStatBoostSequencer instance = wrapper.This;
        bool isAttack = wrapper.GetFieldValue<object>("<>8__1").GetFieldValue<bool>("attackMod");

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.selectionSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        CampfireBoostContext context = new CampfireBoostContext(isAttack);
        List<SelectableCard> cards = instance.selectionSlot.cardSelector.displayedCards;

        Card card = await Neuro.Cards.ChooseCardLoadout(cards, DecisionStrings.CampfireChooseCard, ct, context);

        await NeuroMouse.MoveToAsync(card, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        AllowManualInterruption = false;
        await NeuroMouse.ClickAsync(750, ct);

        // todo: just ask for a bool instead
        // todo: always upgrade if risk-free?

        Description desc = !RunState.Run.survivorsDead ? DecisionStrings.CampfireRiskUpgrade : DecisionStrings.CampfireNoRiskUpgrade;
        InteractableBase chosenButton = await Neuro.Misc.ChooseInteractable(desc, LoadoutState.Calculate(),
            [
                (new CampfireSecondUpgradeAction(), instance.confirmStone),
                (new LeaveAction(), instance.selectionSlot),
            ], ct);
        AutomationManager.Assert(chosenButton, "did not pick on second campfire upgrade");
        await UniTask.WaitUntil(() => instance.confirmStone.Enabled, cancellationToken: ct);
        
        await NeuroMouse.MoveToAsync(chosenButton, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
