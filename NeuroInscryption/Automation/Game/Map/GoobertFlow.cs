using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
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

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.selectionSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        Card card = await Neuro.Cards.ChooseCardLoadout(instance.selectionSlot.cardSelector.displayedCards, DecisionStrings.GoobertChooseCard, ct);

        Debug.Assert(card, "did not pick goobert card");

        await NeuroMouse.MoveToAsync(card, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
