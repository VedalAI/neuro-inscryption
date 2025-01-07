using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
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
        CardStatBoostSequencer instance = enumerator.GetThis<CardStatBoostSequencer>();
        bool isAttack = enumerator.GetFieldValue<object>("<>8__1").GetFieldValue<bool>("attackMod");

        CardInfo destroyedCard = enumerator.GetFieldValue<CardInfo>("<destroyedCard>5__2");
        bool finishedBuffing = enumerator.GetFieldValue<bool>("<finishedBuffing>5__3");
        int numBuffsGiven = enumerator.GetFieldValue<int>("<numBuffsGiven>5__4");

        bool finished = finishedBuffing || destroyedCard != null;
        if (finished) return;

        await Wait.CommonFlowDelay(ct);

        if (!ViewManager.Instance.Is(View.StatBoostSlot))
        {
            CampfireBoostContext context = new CampfireBoostContext(isAttack);

            Card card = await Neuro.Cards.SelectCardFromDeck(instance.selectionSlot, DecisionStrings.CampfireChooseCard, ct,
                additionalContext: context);
            AutomationManager.Assert(card, "did not pick campfire card");

            await Wait.View(View.StatBoostSlot, ct);
        }

        if (numBuffsGiven == 0)
        {
            await NeuroMouse.MoveAndClick(instance.confirmStone, ct,
                msDelayAfterClick: 300); // there's a WaitForSeconds(0.25f) before stats are actually applied
        }

        string decisionQuery = !RunState.Run.survivorsDead ? DecisionStrings.CampfireRiskUpgrade : DecisionStrings.CampfireNoRiskUpgrade;
        bool doSecondUpgrade = await Neuro.Loadout.Choose(new CampfireSecondUpgradeAction(), decisionQuery, ct);
        InteractableBase chosenButton = doSecondUpgrade ? instance.confirmStone : instance.retrieveCardInteractable;

        await UniTask.WaitUntil(() => instance.confirmStone.Enabled, cancellationToken: ct);
        await NeuroMouse.MoveAndClick(chosenButton, ct);
    }
}
