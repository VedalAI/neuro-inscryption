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

public sealed class GoobertFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CopyCardSequencer), nameof(CopyCardSequencer.CopyCardSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(10)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        CopyCardSequencer instance = enumerator.GetThis<CopyCardSequencer>();

        await Wait.CommonFlowDelay(ct);

        if (!ViewManager.Instance.Is(View.CardMergeConfirm))
        {
            Card card = await Neuro.Cards.SelectCardFromDeck(instance.selectionSlot, DecisionStrings.GoobertChooseCard, ct);
            AutomationManager.Assert(card, "did not pick goobert card");

            await Wait.View(View.CardMergeConfirm, ct);
        }

        await NeuroMouse.MoveAndClick(instance.confirmStone, ct);
    }
}
