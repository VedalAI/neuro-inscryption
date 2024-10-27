using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
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

public sealed class DeckTrialFlow : HarmonyAutomationFlow<IEnumerator>
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
            IsValid = FlowHookHelpers.EnumeratorStateIs(7)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        DeckTrialSequencer instance = enumerator.GetThis<DeckTrialSequencer>();

        await Wait.SceneChanged(ct);

        List<SelectableCard> cards = instance.selectableCards;
        await Neuro.Cards.UnflipCards(cards, ct);

        List<DeckTrialData> trials = instance.trialChoices.Zip(cards, DeckTrialData.Create).ToList();
        DeckTrialData trial = await Neuro.Misc.ChooseWithLoadout(trials, DecisionStrings.ChooseDeckTrial, ct);
        await NeuroMouse.MoveAndClick(trial.Card, ct);
    }
}
