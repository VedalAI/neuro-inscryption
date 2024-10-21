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

public sealed class MycologistsFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableCardPair> pairs, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardPairData> PairOptions = pairs.Select(p => SelectableCardPairData.Create(p, state)).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(DuplicateMergeSequencer), nameof(DuplicateMergeSequencer.MergeSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(11)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        DuplicateMergeSequencer instance = target.GetThis<DuplicateMergeSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.selectionSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(((SelectableCardPairArray) instance.selectionSlot.cardSelector).pairs, state);
        state.SetContext(context);

        ChooseAction<SelectableCardPairData> choosePairAction = new(context.PairOptions);

        Decision decision = new(DecisionStrings.MycologistsChoosePair, state);
        decision.AddAction(choosePairAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(choosePairAction, out SelectableCardPairData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(selected.Pair, 200, ct);
        await NeuroMouse.ClickAsync(500, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
