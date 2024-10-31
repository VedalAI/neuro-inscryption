using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using NeuroInscryption.Utilities;
using UnityEngine;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal sealed class Cards
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class LoadoutContext(IEnumerable<SelectableCard> cards, object? additionalContext = null)
        : Loadout.ItemsContext<SelectableCardData>(cards.Select(SelectableCardData.Create), additionalContext)
    {
        protected override IEnumerable<SigilData> RelevantSigils => Items.SelectMany(c => c.Card!.RelevantSigils);
        protected override IEnumerable<TraitData> RelevantTraits => Items.SelectMany(c => c.Card!.Traits);
    }

    public async UniTask<Card> ChooseAddToDeck(CardChoicesSequencer choicer, CancellationToken ct = default)
    {
        List<SelectableCard> cards = choicer.selectableCards;

        await UnflipCards(cards, ct);

        LoadoutState state = LoadoutState.Calculate();
        LoadoutContext context = new(cards);
        state.SetContext(context);

        CardSingleChoicesSequencer? rerollable = choicer as CardSingleChoicesSequencer;
        MainInputInteractable clover = rerollable != null ? rerollable.rerollInteractable : null!;
        RerollAction rerollAction = new(clover);
        ChooseAction<SelectableCardData> chooseAction = new(context.Items);

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
            await NeuroMouse.MoveAndClick(clover, ct);
            // choicer sets this back to false when ready for input again
            await UniTask.WaitUntil(() => !rerollable!.choicesRerolled, cancellationToken: ct);
            return await ChooseAddToDeck(choicer, ct);
        }

        action.ThrowInvalid();
        throw new InvalidOperationException($"Unreachable in {nameof(ChooseAddToDeck)} after ThrowInvalid");
    }

    public async UniTask UnflipCards(IEnumerable<SelectableCard> cards, CancellationToken ct = default)
    {
        await Wait.ViewUnlocked(ct);
        foreach (SelectableCard card in cards.Where(c => c.Flipped))
        {
            await NeuroMouse.MoveAndClick(card, ct);
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
        AutomationManager.Assert(cards.Count > 0, "Nothing to pick from");

        await UnflipCards(cards, ct);

        if (cards.Count == 1) return cards.First();

        List<SelectableCardData> data = cards.Select(SelectableCardData.Create).ToList();
        SelectableCardData selected = await Neuro.Loadout.ChooseItem(data, description, ct, additionalContext);
        return selected.TargetCard;
    }

    public async UniTask<Card> SelectCardFromDeck(SelectCardFromDeckSlot slot, Description description, CancellationToken ct = default,
        Func<SelectableCard, bool>? filter = null, object? additionalContext = null,
        float cardPileSpawnTimeMultiplier = 1f, float cardPileDespawnTimeMultiplier = 0.5f)
    {
        await Wait.CameraStable(ct);

        if (!ViewManager.Instance.Is(View.DeckSelection))
        {
            await NeuroMouse.MoveAndClick(slot, ct,
                msDelayAfterClick: Mathf.CeilToInt(Wait.CardPileFullSpawnTimeMs * cardPileSpawnTimeMultiplier));
            await Wait.View(View.DeckSelection, ct);
        }

        List<SelectableCard> cards = slot.cardSelector.displayedCards;
        if (filter is { }) cards = cards.Where(filter).ToList();

        AutomationManager.Assert(cards.Count > 0, "No selectable cards found" + (filter is { } ? ", check filter" : ""));

        Card card = await Neuro.Cards.ChooseCardLoadout(cards, description, ct, additionalContext);
        await NeuroMouse.MoveAndClick(card, ct,
            msDelayAfterClick: Mathf.CeilToInt(Wait.CardPileFullSpawnTimeMs * cardPileDespawnTimeMultiplier));

        await UniTask.WaitUntil(() => !ViewManager.Instance.Is(View.DeckSelection), cancellationToken: ct);
        await Wait.CameraStable(ct);

        return card ? card : slot.Card; // behaviour varies
    }
}
