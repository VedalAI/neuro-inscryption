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

public sealed class PackFullFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(GainConsumablesSequencer), nameof(GainConsumablesSequencer.FullConsumablesSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(5)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        GainConsumablesSequencer instance = enumerator.GetThis<GainConsumablesSequencer>();

        await Wait.CommonFlowDelay(ct);

        await NeuroMouse.MoveAndClick(instance.ratCard, ct);
    }
}
