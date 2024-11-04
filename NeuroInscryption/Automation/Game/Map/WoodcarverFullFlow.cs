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

public sealed class WoodcarverFullFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.TotemSlotsFullSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(4),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        SelectableCard card = enumerator.GetFieldValue<SelectableCard>("<card>5__2");
        await NeuroMouse.MoveAndClick(card, ct);
    }
}
