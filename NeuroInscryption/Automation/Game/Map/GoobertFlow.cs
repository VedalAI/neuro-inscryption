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

        await Wait.SceneChanged(ct);

        if (!ViewManager.Instance.Is(View.CardMergeConfirm))
        {
            if (ViewManager.Instance.Is(View.Default))
            {
                await NeuroMouse.MoveAndClick(instance.selectionSlot, ct,
                    delayAfterClick: Wait.CardPileFullSpawnTimeMs);
            }

            await Wait.View(View.DeckSelection, ct);

            Card card = await Neuro.Cards.ChooseCardLoadout(instance.selectionSlot.cardSelector.displayedCards, DecisionStrings.GoobertChooseCard, ct);
            AutomationManager.Assert(card, "did not pick goobert card");
            await NeuroMouse.MoveAndClick(card, ct,
                delayAfterClick: Wait.CardPileFullSpawnTimeMs);
        }

        await Wait.View(View.CardMergeConfirm, ct);

        await NeuroMouse.MoveAndClick(instance.confirmStone, ct);
    }
}
