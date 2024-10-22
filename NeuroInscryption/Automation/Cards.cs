using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Input.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NeuroInscryption.Automation;

internal class Cards
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class ChooseCardContext(IEnumerable<SelectableCard> cards, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => CardOptions.SelectMany(c => c.Card!.RelevantSigils);
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => CardOptions.SelectMany(c => c.Card!.Traits);
    }

    public async UniTask<SelectableCard> ChooseCard(CardChoicesSequencer choicer, CancellationToken ct = default)
    {
        await UnflipCards(choicer);

        var cards = choicer.selectableCards;
        LoadoutState state = LoadoutState.Calculate();
        ChooseCardContext context = new(cards, state);
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
            Debug.Assert(clover, $"Ended up in reroll path on non-rerollable choicer {choicer.GetType().Name} - schema enforcement fail?");
            await NeuroMouse.MoveToAsync(clover, 200, ct);
            await NeuroMouse.ClickAsync(ct);
            //await UniTask.Delay(100, cancellationToken: ct);
            // choicer sets this back to false when ready for input again
            await UniTask.WaitUntil(() => !rerollable.choicesRerolled, cancellationToken: ct);
            return await ChooseCard(choicer, ct);
        }

        action.ThrowInvalid();
        throw new System.InvalidOperationException($"Unreachable in {nameof(ChooseCard)} after ThrowInvalid");
    }

    public async UniTask UnflipCards(CardChoicesSequencer choicer, CancellationToken ct = default)
    {
        await InputHelpers.WaitUntilViewUnlocked(ct);
        foreach (SelectableCard card in choicer.selectableCards.Where(c => c.Flipped))
        {
            await NeuroMouse.MoveToAsync(card, 200, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        await UniTask.Delay(200, cancellationToken: ct);
    }
}
