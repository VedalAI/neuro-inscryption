using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ProspectorPickupFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoulderChoiceSequencer), nameof(BoulderChoiceSequencer.RewardSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 4
        }
    ];

    protected override async UniTask ExecuteAsync(object target, CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
