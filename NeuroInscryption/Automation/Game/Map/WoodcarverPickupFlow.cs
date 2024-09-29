using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverPickupFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.BuildPhase))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 14,
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        BuildTotemSequencer instance = enumerator.GetEnumeratorThis<BuildTotemSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.completeTotemSlot, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
