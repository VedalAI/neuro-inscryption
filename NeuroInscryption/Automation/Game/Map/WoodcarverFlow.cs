using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverFlow : AutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context1(IEnumerable<SelectableItemSlot> items)
    {
        public readonly IReadOnlyList<TotemPieceData> TotemPieceOptions = items.Select(slot => TotemPieceData.Create(slot)!).ToList();
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context2(TotemData? totem)
    {
        public readonly TotemData? LastTotem = totem;
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.NewPiecePhase))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 4,
        }
    ];

    public override bool CanInterruptItself => false;

    public override void ResetState()
    {
        AllowManualInterruption = true;
    }

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        BuildTotemSequencer instance = enumerator.GetEnumeratorThis<BuildTotemSequencer>();

        Context1 context = new(instance.slots);
        LoadoutState state = LoadoutState.CalculateWithContext(context);

        ChooseAction<TotemPieceData> chooseAction = new(context.TotemPieceOptions, true);

        Decision decision = new(DecisionStrings.ChooseTotemPiece, state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out TotemPieceData? piece)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(piece.Slot!, 300, ct);
        AllowManualInterruption = false;
        await NeuroMouse.ClickAsync(ct);

        await HandleTotemBuildAsync(instance, state, ct);
    }

    private async UniTask HandleTotemBuildAsync(BuildTotemSequencer instance, LoadoutState oldState, CancellationToken ct)
    {
        bool canBuildTotem = RunState.Run.totemTops.Count > 0 && RunState.Run.totemBottoms.Count > 0;
        bool autoBuildTotem = RunState.Run.totemTops.Count == 1 && RunState.Run.totemBottoms.Count == 1;

        if (!canBuildTotem) return;

        // TODO: make sure dialogue doesnt break this
        await UniTask.Delay(1000, cancellationToken: ct);

        if (!autoBuildTotem)
        {
            Context2 context = new(oldState.Totem);
            LoadoutState state = LoadoutState.CalculateWithContext(context); // need new state since we have new totem parts

            BuildTotemAction buildTotemAction = new(instance.inventorySlots.Where(slot => slot.Item != null).Select(TotemPieceData.Create)!);

            Decision decision = new(DecisionStrings.BuildTotem, state);
            decision.AddAction(buildTotemAction);

            Action action = await decision.SendAsync(ct);

            if (!action.Match(buildTotemAction, out BuildTotemAction.Response response)) action.ThrowInvalid();

            await NeuroMouse.MoveToAsync(response.Top.Slot!, 200, ct);
            await NeuroMouse.ClickAsync(ct);
            await NeuroMouse.MoveToAsync(response.Bottom.Slot!, 200, ct);
            await NeuroMouse.ClickAsync(1000, ct);
        }

        await NeuroMouse.MoveToAsync(instance.completeTotemSlot, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
