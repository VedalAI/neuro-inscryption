using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class BoneAltarFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardRemoveSequencer), nameof(CardRemoveSequencer.RemoveSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(7),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        CardRemoveSequencer instance = target.GetThis<CardRemoveSequencer>();

        await Wait.CommonFlowDelay(ct);

        if (!ViewManager.Instance.Is(View.CardMergeConfirm))
        {
            Card card = await Neuro.Cards.SelectCardFromDeck(instance.sacrificeSlot, DecisionStrings.BoneAltarChooseCard, ct);
            AutomationManager.Assert(card, "did not pick bone altar card");

            await Wait.View(View.CardMergeConfirm, ct);
        }

        if (!instance.confirmStone.SelectionConfirmed)
        {
            await NeuroMouse.MoveAndClick(instance.confirmStone, ct);
            await UniTask.WaitUntil(() => !instance.sacrificeSlot.Card, cancellationToken: ct);
        }

        SelectableCard? boonCard = null;
        // used to click the slot but the boon card isn't actually placed on the slot, it's just moved to the same position
        await UniTask.WaitUntil(checkBoonCard, cancellationToken: ct);
        await NeuroMouse.MoveAndClick(boonCard!, ct);

        return;

        bool checkBoonCard()
        {
            boonCard = target.GetFieldValue<SelectableCard>("<boonCard>5__6");
            return boonCard != null;
        }
    }

}
