using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class CampfireFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context1(IEnumerable<SelectableCard> cards, IGameState state, bool isAttack) : ILoadoutContext
    {
        public readonly string BoostType = isAttack ? DecisionStrings.CampfireAttackBoost : DecisionStrings.CampfireHealthBoost;
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context2(CardData card, bool isAttack) : ILoadoutContext
    {
        public readonly string BoostType = isAttack ? DecisionStrings.CampfireAttackBoost : DecisionStrings.CampfireHealthBoost;
        public readonly CardData ChosenCardToUpgrade = card;

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
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

        LoadoutState state = LoadoutState.Calculate();
        Context1 context = new(instance.selectionSlot.cardSelector.displayedCards, state, isAttack);
        state.SetContext(context);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.CardOptions, true);

        Decision decision = new(DecisionStrings.CampfireChooseCard, state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(selected.TargetCard, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        AllowManualInterruption = false;
        await NeuroMouse.ClickAsync(750, ct);

        await HandleSecondUpgradeAsync(instance, isAttack, state, selected.Card!, ct);
    }

    private async UniTask HandleSecondUpgradeAsync(CardStatBoostSequencer instance, bool isAttack, LoadoutState state, CardData oldCardData, CancellationToken ct)
    {
        Context2 context = new(CardData.Create(oldCardData.TargetCardInfo, state), isAttack);
        state.SetContext(context);

        CampfireSecondUpgradeAction campfireSecondUpgradeAction = new();
        LeaveAction leaveAction = new(false);

        Decision decision = new(!RunState.Run.survivorsDead ? DecisionStrings.CampfireRiskUpgrade : DecisionStrings.CampfireNoRiskUpgrade, state);
        decision.AddAction(campfireSecondUpgradeAction);
        decision.AddAction(leaveAction);

        Action action = await decision.SendAsync(ct);

        await UniTask.WaitUntil(() => instance.confirmStone.Enabled, cancellationToken: ct);

        if (action.Match(campfireSecondUpgradeAction))
        {
            await NeuroMouse.MoveToAsync(instance.confirmStone, 100, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else if (action.Match(leaveAction))
        {
            await NeuroMouse.MoveToAsync(instance.selectionSlot, 200, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
