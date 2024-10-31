using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class MycologistsFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(DuplicateMergeSequencer), nameof(DuplicateMergeSequencer.MergeSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(11)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        DuplicateMergeSequencer instance = target.GetThis<DuplicateMergeSequencer>();

        await Wait.CommonFlowDelay(ct);

        if (!ViewManager.Instance.Is(View.CardMergeConfirm))
        {
            if (ViewManager.Instance.Is(View.Default))
            {
                await NeuroMouse.MoveAndClick(instance.selectionSlot, ct,
                    msDelayAfterClick: Wait.CardPileFullSpawnTimeMs);

                await Wait.View(View.DeckSelection, ct);
            }

            SelectableCardPairArray selector = (SelectableCardPairArray)instance.selectionSlot.cardSelector;
            List<SelectableCardPairData> pairs = selector.pairs.Select(SelectableCardPairData.Create).ToList();

            SelectableCardPairData pair = await Neuro.Loadout.ChooseItem(pairs, DecisionStrings.MycologistsChoosePair, ct);
            AutomationManager.Assert(pair is {} && pair.Pair, "did not pick pair for mycologists");

            await NeuroMouse.MoveAndClick(pair.Pair, ct);

            await Wait.View(View.CardMergeConfirm, ct);
        }

        await NeuroMouse.MoveAndClick(instance.confirmStone, ct, 250);
    }
}
