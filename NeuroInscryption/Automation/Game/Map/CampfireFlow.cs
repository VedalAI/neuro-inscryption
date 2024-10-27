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
using NeuroInscryption.Utilities;
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

        CardInfo destroyedCard = wrapper.GetFieldValue<CardInfo>("<destroyedCard>5__2");
        bool finishedBuffing = wrapper.GetFieldValue<bool>("<finishedBuffing>5__3");

        bool finished = finishedBuffing || destroyedCard != null;
        if (finished) return;

        await Wait.CameraStable(ct);

        int numBuffsGiven = wrapper.GetFieldValue<int>("<numBuffsGiven>5__4");

        bool cardSelected = instance.selectionSlot.Card != null;
        if (!ViewManager.Instance.Is(View.StatBoostSlot))
        {
            if (ViewManager.Instance.Is(View.Default))
            {
                await NeuroMouse.MoveAndClick(instance.selectionSlot, ct,
                    delayAfterClick: Wait.CardPileFullSpawnTimeMs);
            }

            await Wait.View(View.DeckSelection, ct);

            CampfireBoostContext context = new CampfireBoostContext(isAttack);
            List<SelectableCard> cards = instance.selectionSlot.cardSelector.displayedCards;

            Card card = await Neuro.Cards.ChooseCardLoadout(cards, DecisionStrings.CampfireChooseCard, ct, context);

            await NeuroMouse.MoveAndClick(card, ct,
                delayAfterClick: Wait.CardPileFullSpawnTimeMs); // card pile takes the same time to despawn
        }

        await Wait.View(View.StatBoostSlot, ct);

        if (numBuffsGiven == 0)
        {
            await NeuroMouse.MoveAndClick(instance.confirmStone, ct,
                delayAfterClick: 300); // there's a WaitForSeconds(0.25f) before stats are actually applied
        }

        // todo: just ask for a bool instead
        Description desc = !RunState.Run.survivorsDead ? DecisionStrings.CampfireRiskUpgrade : DecisionStrings.CampfireNoRiskUpgrade;
        InteractableBase chosenButton = await Neuro.Misc.ChooseInteractable(desc, LoadoutState.Calculate(),
        [
            (new CampfireSecondUpgradeAction(), instance.confirmStone),
            (new LeaveAction(), instance.selectionSlot.Card!),
        ], ct);
        AutomationManager.Assert(chosenButton, "did not pick on second campfire upgrade");

        await UniTask.WaitUntil(() => instance.confirmStone.Enabled, cancellationToken: ct);

        await NeuroMouse.MoveAndClick(chosenButton, ct);
    }
}
