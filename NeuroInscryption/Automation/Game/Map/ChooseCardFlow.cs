using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ChooseCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardSingleChoicesSequencer), nameof(CardSingleChoicesSequencer.CardSelectionSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(11), // this used to check (enumerator.This.choicesRerolled: false) but that's always false at state 11
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(typeof(RareCardChoicesSequencer).GetMethod(nameof(RareCardChoicesSequencer.ChooseRareCard))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(8),
        },
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        EnumeratorWrapper<CardSingleChoicesSequencer> wrapper = new(enumerator);
        CardSingleChoicesSequencer instance = wrapper.This;

        await UniTask.Delay(500, cancellationToken: ct);

        Card card = await Neuro.Cards.ChooseAddToDeck(instance, ct);

        await NeuroMouse.MoveToAsync(card, 200, ct);
        AllowManualInterruption = false;
        await NeuroMouse.ClickAsync(ct);

        if (instance is RareCardChoicesSequencer) return;
        
        CardChoicesType choiceType = wrapper.GetFieldValue<CardChoicesNodeData>("<choicesData>5__2").choicesType;
        if (choiceType != CardChoicesType.Random)
        {
            await UniTask.Delay(150, cancellationToken: ct);
            await NeuroMouse.ClickAsync(ct);
        }
    }
}
