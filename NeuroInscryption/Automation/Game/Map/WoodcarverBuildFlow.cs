using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Helpers;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverBuildFlow : HarmonyAutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context : ILoadoutContext
    {
        public readonly TotemData? LastTotem = WoodcarverTotemTracker.LastTotem;

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.BuildPhase))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 12 && RunState.Run.totemTops.Count + RunState.Run.totemBottoms.Count > 2
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        BuildTotemSequencer instance = enumerator.GetEnumeratorThis<BuildTotemSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new();
        state.SetContext(context);

        BuildTotemAction buildTotemAction = new(instance.inventorySlots.Where(slot => slot.Item != null).Select(TotemPieceData.Create)!);

        Decision decision = new(DecisionStrings.BuildTotem, state);
        decision.AddAction(buildTotemAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(buildTotemAction, out BuildTotemAction.Response response)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(response.Top.Slot!, 200, ct);
        AllowManualInterruption = false;
        await NeuroMouse.ClickAsync(ct);
        await NeuroMouse.MoveToAsync(response.Bottom.Slot!, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);
    }
}
