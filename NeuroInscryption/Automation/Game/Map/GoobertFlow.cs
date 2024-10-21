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

public sealed class GoobertFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableCard> cards, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CopyCardSequencer), nameof(CopyCardSequencer.CopyCardSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(10)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        CopyCardSequencer instance = enumerator.GetThis<CopyCardSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.selectionSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.selectionSlot.cardSelector.displayedCards, state);
        state.SetContext(context);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.CardOptions);

        Decision decision = new(DecisionStrings.GoobertChooseCard, state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(selected.TargetCard, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
