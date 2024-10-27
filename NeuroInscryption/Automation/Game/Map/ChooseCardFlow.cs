using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
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
        CardSingleChoicesSequencer instance = enumerator.GetThis<CardSingleChoicesSequencer>();

        await Wait.SceneChanged(ct);

        if (instance.chosenReward == null)
        {
            Card card = await Neuro.Cards.ChooseAddToDeck(instance, ct);
            AutomationManager.Assert(card, "did not pick card");
            await NeuroMouse.MoveAndClick(card, ct);
        }

        if (instance is RareCardChoicesSequencer) return;
        CardChoicesType choiceType = enumerator.GetFieldValue<CardChoicesNodeData>("<choicesData>5__2").choicesType;
        if (choiceType == CardChoicesType.Random) return;

        await UniTask.WaitUntil(() => instance.chosenReward != null, cancellationToken: ct);
        await NeuroMouse.MoveAndClick(instance.chosenReward!, ct);
    }
}
