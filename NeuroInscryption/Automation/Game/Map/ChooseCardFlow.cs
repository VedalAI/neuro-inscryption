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

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ChooseCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableCard> cards, CardChoicesType type, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        public readonly string? Note = type switch
        {
            CardChoicesType.Cost => "You will receive a random card with the cost equal to the once you choose.",
            CardChoicesType.Tribe => "You will receive a random card that belongs to the tribe that you choose.",
            _ => null
        };

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => CardOptions.SelectMany(c => c.Card!.RelevantSigils);
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => CardOptions.SelectMany(c => c.Card!.Traits);
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardSingleChoicesSequencer), nameof(CardSingleChoicesSequencer.CardSelectionSequence))),
            IsValid = context =>
            {
                EnumeratorWrapper<CardSingleChoicesSequencer> enumerator = new(context.GetInstance<IEnumerator>()!);
                return enumerator.State == 11 && !enumerator.This.choicesRerolled;
            }
        }
        // TODO: boss rares
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        EnumeratorWrapper<CardSingleChoicesSequencer> wrapper = new(enumerator);
        CardSingleChoicesSequencer instance = wrapper.This;
        CardChoicesType choiceType = wrapper.GetFieldValue<CardChoicesNodeData>("<choicesData>5__2").choicesType;

        await UniTask.Delay(500, cancellationToken: ct);

        foreach (SelectableCard card in instance.selectableCards.Where(c => c.Flipped))
        {
            bool hasTutorial = card.Info && !string.IsNullOrEmpty(card.Info.description) && !ProgressionData.IntroducedCard(card.Info);

            await NeuroMouse.MoveToAsync(card, 200, ct);
            await NeuroMouse.ClickAsync(ct);

            if (hasTutorial)
            {
                await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Locked, cancellationToken: ct);
                await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);
                await UniTask.Delay(350, cancellationToken: ct);
            }
        }

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(instance.selectableCards, choiceType, state);
        state.SetContext(context);

        RerollAction rerollAction = new(instance.rerollInteractable);
        ChooseAction<SelectableCardData> chooseAction = new(context.CardOptions, !rerollAction.Applicable);

        Decision decision = new(DecisionStrings.ChooseCard, state);
        decision.AddAction(chooseAction);
        decision.AddAction(rerollAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(chooseAction, out SelectableCardData? cardData))
        {
            await NeuroMouse.MoveToAsync(cardData.TargetCard, 200, ct);
            AllowManualInterruption = false;
            await NeuroMouse.ClickAsync(ct);

            if (choiceType != CardChoicesType.Random)
            {
                await UniTask.Delay(750, cancellationToken: ct);
                await NeuroMouse.ClickAsync(ct);
            }
        }
        else if (action.Match(rerollAction))
        {
            await NeuroMouse.MoveToAsync(instance.rerollInteractable, 200, ct);
            AllowManualInterruption = false;
            await NeuroMouse.ClickAsync(ct);
            await UniTask.WaitUntil(() => instance.choicesRerolled, cancellationToken: ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
