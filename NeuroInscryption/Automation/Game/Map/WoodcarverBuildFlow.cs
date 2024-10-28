using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Helpers;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverBuildFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.BuildPhase))),
            IsValid = context => context.GetInstance<IEnumerator>()!.GetState() == 12 && RunState.Run.totemTops.Count + RunState.Run.totemBottoms.Count > 2
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        BuildTotemSequencer instance = enumerator.GetThis<BuildTotemSequencer>();

        await Wait.CommonFlowDelay(ct);

        BuildTotemAction buildTotemAction = new(instance.inventorySlots.Where(slot => slot.Item != null).Select(TotemPieceData.Create)!);
        BuildTotemAction.Response response = await Neuro.Loadout.Choose(buildTotemAction, DecisionStrings.BuildTotem, ct,
            new { WoodcarverTotemTracker.LastTotem }
        );

        // edge case: with a bottom piece previously selected, the bottom choice is effectively discarded
        if (instance.selectedTop != response.Top.Slot)
            await NeuroMouse.MoveAndClick(response.Top.Slot!, ct);
        if (instance.selectedBottom != response.Bottom.Slot)
            await NeuroMouse.MoveAndClick(response.Bottom.Slot!, ct);
    }
}
