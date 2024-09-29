using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class BoneAltarPickupFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardRemoveSequencer), nameof(CardRemoveSequencer.RemoveSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 19
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(object target, CancellationToken ct)
    {
        CardRemoveSequencer instance = target.GetEnumeratorThis<CardRemoveSequencer>();

        await NeuroMouse.MoveToAsync(instance.sacrificeSlot, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
