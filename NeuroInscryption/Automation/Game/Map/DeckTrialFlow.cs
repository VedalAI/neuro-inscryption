using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
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

public sealed class DeckTrialFlow : AutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<DeckTrialSequencer.Trial> trials, IEnumerable<SelectableCard> cards) : ILoadoutContext
    {
        public readonly IReadOnlyList<DeckTrialData> TrialOptions = trials.Zip(cards, DeckTrialData.Create).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(DeckTrialSequencer), nameof(DeckTrialSequencer.DeckTrialSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 7
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        DeckTrialSequencer instance = enumerator.GetEnumeratorThis<DeckTrialSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        foreach (SelectableCard card in instance.selectableCards.Where(c => c.Flipped))
        {
            await NeuroMouse.MoveToAsync(card, 100, ct);
            await NeuroMouse.ClickAsync(ct);

            await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Locked, cancellationToken: ct);
            await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);
            await UniTask.Delay(150, cancellationToken: ct);
        }

        Context context = new(instance.trialChoices, instance.selectableCards);
        LoadoutState state = LoadoutState.Calculate();
        state.SetContext(context);

        ChooseAction<DeckTrialData> chooseAction = new(context.TrialOptions, true);

        Decision decision = new(DecisionStrings.ChooseDeckTrial, state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out DeckTrialData? trial)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(trial.Card, 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
