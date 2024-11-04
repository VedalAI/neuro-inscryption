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

public sealed class ProspectorPickupFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoulderChoiceSequencer), nameof(BoulderChoiceSequencer.RewardSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(4)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        SelectableCard rewardCard = target.GetFieldValue<SelectableCard>("<rewardCard>5__2");
        await NeuroMouse.MoveAndClick(rewardCard, ct);
    }
}
