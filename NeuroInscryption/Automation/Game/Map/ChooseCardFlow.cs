using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ChooseCardFlow : AutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(List<SelectableCard> cards, CardChoicesType type, IGameState state)
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        public readonly string? Note = type switch
        {
            CardChoicesType.Cost => "You will receive a random card with the cost equal to the once you choose.",
            CardChoicesType.Tribe => "You will receive a random card that belongs to the tribe that you choose.",
            _ => null
        };
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardSingleChoicesSequencer), nameof(CardSingleChoicesSequencer.CardSelectionSequence))),
            IsValid = context =>
            {
                object enumerator = context.GetInstance()!;
                return enumerator.GetEnumeratorState() == 11 && enumerator.GetEnumeratorThis<CardSingleChoicesSequencer>().choicesRerolled == false;
            }
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct);

        CardSingleChoicesSequencer instance = enumerator.GetEnumeratorThis<CardSingleChoicesSequencer>();
        CardChoicesType choiceType = enumerator.GetEnumeratorField<CardChoicesNodeData>("<choicesData>5__2").choicesType;

        foreach (SelectableCard card in instance.selectableCards.Where(c => c.Flipped))
        {
            bool hasTutorial = card.Info && !string.IsNullOrEmpty(card.Info.description) && !ProgressionData.IntroducedCard(card.Info);

            await NeuroMouse.MoveToAsync(card, 200, ct);
            await NeuroMouse.ClickAsync(ct);

            if (hasTutorial)
            {
                await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Locked, cancellationToken: ct);
                await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);
                await UniTask.Delay(500, cancellationToken: ct);
            }
        }

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.selectableCards, choiceType, state);
        state.SetContext(context);

        ChooseCardAction chooseCardAction = new(context.CardOptions);
        RerollAction rerollAction = new(instance.rerollInteractable);

        Decision decision = new(DecisionIds.ChooseCard, state, DecisionStrings.ChooseCard);
        decision.AddAction(chooseCardAction);
        decision.AddAction(rerollAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(chooseCardAction, out SelectableCardData? cardData))
        {
            await NeuroMouse.MoveToAsync(cardData.TargetCard, 200, ct, mouseSpeedMultiplier: 2);
            await NeuroMouse.ClickAsync(ct);

            if (choiceType != CardChoicesType.Random)
            {
                await UniTask.Delay(750, cancellationToken: ct);
                await NeuroMouse.ClickAsync(ct);
            }
        }
        else if (action.Match(rerollAction))
        {
            await NeuroMouse.MoveToAsync(instance.rerollInteractable, 500, ct);
            await NeuroMouse.ClickAsync(ct);
            await UniTask.WaitUntil(() => instance.choicesRerolled, cancellationToken: ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
