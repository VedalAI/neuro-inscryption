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
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class SacrificeFlow : HarmonyAutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableCard> cards, IGameState state, CardMergeSequencer instance) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Where(card => instance.GetValidCardsForHost(card.Info).Any()).Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardMergeSequencer), nameof(CardMergeSequencer.MergeSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 12
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        CardMergeSequencer instance = enumerator.GetEnumeratorThis<CardMergeSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.sacrificeSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.sacrificeSlot.cardSelector.displayedCards, state, instance);
        state.SetContext(context);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.CardOptions, true);

        Decision decision = new(DecisionStrings.Sacrifice, state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(selected.TargetCard, 200, ct);
        await NeuroMouse.ClickAsync(ct);

        AutomationManager.StartFlowManually<SacrificeEmpowerFlow, CardMergeSequencer>(instance);
    }
}
