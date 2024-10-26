using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
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

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.sacrificeSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        List<SelectableCard> cards = instance.sacrificeSlot.cardSelector.displayedCards;
        Card card = await Neuro.Cards.ChooseCardLoadout(cards, DecisionStrings.BoneAltarChooseCard, ct: ct);
        Debug.Assert(card, "did not pick bone altar card");

        await NeuroMouse.MoveToAsync(card, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
