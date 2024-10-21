using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverChooseFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableItemSlot> items) : ILoadoutContext
    {
        public readonly IReadOnlyList<TotemPieceData> TotemPieceOptions = items.Select(slot => TotemPieceData.Create(slot)!).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.NewPiecePhase))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(4),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        BuildTotemSequencer instance = enumerator.GetThis<BuildTotemSequencer>();

        Context context = new(instance.slots);
        LoadoutState state = LoadoutState.Calculate();
        state.SetContext(context);

        ChooseAction<TotemPieceData> chooseAction = new(context.TotemPieceOptions);

        Decision decision = new(DecisionStrings.ChooseTotemPiece, state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out TotemPieceData? piece)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(piece.Slot!, 300, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
