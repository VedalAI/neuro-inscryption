using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Input.Helpers;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation;

internal class Cards
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class LoadoutContext(IEnumerable<SelectableCard> cards) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select(SelectableCardData.Create).ToList();
        public object? AdditionalContext { get; init; }

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => CardOptions.SelectMany(c => c.Card!.RelevantSigils);
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => CardOptions.SelectMany(c => c.Card!.Traits);
    }

    public async UniTask<Card> ChooseAddToDeck(CardChoicesSequencer choicer, CancellationToken ct = default)
    {
        List<SelectableCard> cards = choicer.selectableCards;

        await UnflipCards(cards, ct);

        LoadoutState state = LoadoutState.Calculate();
        LoadoutContext context = new(cards);
        state.SetContext(context);

        CardSingleChoicesSequencer rerollable = (choicer as CardSingleChoicesSequencer)!; // day 999 of hating the way unity works with nulls
        MainInputInteractable clover = rerollable ? rerollable.rerollInteractable : null!;
        RerollAction rerollAction = new(clover);
        ChooseAction<SelectableCardData> chooseAction = new(context.CardOptions);

        Decision decision = new(DecisionStrings.ChooseCard, state);
        decision.AddAction(chooseAction);
        decision.AddAction(rerollAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(chooseAction, out SelectableCardData? cardData))
        {
            Plugin.Logger.LogDebug($"Picked {cardData.Identifier}");
            return cardData.TargetCard;
        }
        if (action.Match(rerollAction))
        {
            AutomationManager.Assert(clover, $"Ended up in reroll path on non-rerollable choicer {choicer.GetType().Name} - schema enforcement fail?");
            await NeuroMouse.MoveToAsync(clover, 200, ct);
            await NeuroMouse.ClickAsync(ct);
            //await UniTask.Delay(100, cancellationToken: ct);
            // choicer sets this back to false when ready for input again
            await UniTask.WaitUntil(() => !rerollable.choicesRerolled, cancellationToken: ct);
            return await ChooseAddToDeck(choicer, ct);
        }

        action.ThrowInvalid();
        throw new InvalidOperationException($"Unreachable in {nameof(ChooseAddToDeck)} after ThrowInvalid");
    }

    public async UniTask UnflipCards(IEnumerable<SelectableCard> cards, CancellationToken ct = default)
    {
        await InputHelpers.WaitUntilViewUnlocked(ct);
        foreach (SelectableCard card in cards.Where(c => c.Flipped))
        {
            await NeuroMouse.MoveToAsync(card, 200, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        await UniTask.Delay(200, cancellationToken: ct);
    }

    /// <summary>
    /// Pick a card from the given <paramref name="cards"/> in the <see cref="LoadoutState">loadout context</see>.
    /// </summary>
    /// <param name="description">Description to send. This should describe what kind of choice is being made.</param>
    /// <param name="additionalContext">Any additional context to include in the state. This can be an anonymous object.</param>
    /// <example>
    /// <code>
    /// Card cardToGloopBlurb = await Neuro.Cards.ChooseCardLoadout(cards,
    ///     new Description("choice_gloop_blurb", "Choose a card to gloop blurb. The card's immediate family will be murgled."),
    ///     new { MurgleInsensity = 5 }
    /// );
    /// </code>
    /// </example>
    public async UniTask<Card> ChooseCardLoadout(IReadOnlyCollection<SelectableCard> cards, Description description, CancellationToken ct = default, object? additionalContext = null)
    {
        await UnflipCards(cards, ct);

        LoadoutState state = LoadoutState.Calculate();
        LoadoutContext context = new(cards) { AdditionalContext = additionalContext };
        state.SetContext(context);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.CardOptions);
        Decision decision = new(description, state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        return selected.TargetCard;
    }
}
