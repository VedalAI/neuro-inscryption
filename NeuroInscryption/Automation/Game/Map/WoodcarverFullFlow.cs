using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverFullFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.TotemSlotsFullSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 4,
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        SelectableCard card = enumerator.GetEnumeratorField<SelectableCard>("<card>5__2");

        await UniTask.Delay(500, cancellationToken: ct);
        await NeuroMouse.MoveToAsync(card, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
