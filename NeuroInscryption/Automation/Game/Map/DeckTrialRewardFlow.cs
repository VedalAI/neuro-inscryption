using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class DeckTrialRewardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(DeckTrialSequencer), nameof(DeckTrialSequencer.DeckTrialSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(27)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        DeckTrialSequencer instance = enumerator.GetThis<DeckTrialSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        SelectableCard cardData = await Neuro.Cards.ChooseAddToDeck(instance, ct);

        await NeuroMouse.MoveToAsync(cardData, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
