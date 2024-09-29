using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class SacrificeEmpowerFlow : AutomationFlow<CardMergeSequencer>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(SelectableCard sacrifice, IEnumerable<SelectableCard> cards, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<SigilData> SigilsToBeAdded = sacrifice.Info.Abilities.Select(SigilData.Create).ToList();
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    protected override async UniTask ExecuteAsync(CardMergeSequencer instance, CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.hostSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.sacrificeSlot.Card, instance.sacrificeSlot.cardSelector.displayedCards, state);
        state.SetContext(context);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.CardOptions, true);

        Decision decision = new(DecisionStrings.SacrificeEmpower.Format(CardData.Create(instance.sacrificeSlot.Card.Info, state).GetDescription()), state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(selected.TargetCard, 200, ct);
        await NeuroMouse.ClickAsync(500, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
