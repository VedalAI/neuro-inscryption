using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class PackFullFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(GainConsumablesSequencer), nameof(GainConsumablesSequencer.FullConsumablesSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 5
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        GainConsumablesSequencer instance = enumerator.GetEnumeratorThis<GainConsumablesSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);
        await NeuroMouse.MoveToAsync(instance.ratCard, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
