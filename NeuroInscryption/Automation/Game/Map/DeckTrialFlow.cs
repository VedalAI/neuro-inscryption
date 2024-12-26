using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class DeckTrialFlow : HarmonyAutomationFlow<IEnumerator>
{
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
        EnumeratorWrapper<DeckTrialSequencer> wrapper = new(enumerator);
        DeckTrialSequencer instance = wrapper.This;

        await Wait.CommonFlowDelay(ct);

        // i'm either very smart or very very dumb
        // either way - we'll need to tell neuro whether she passed the trial :)
        if (wrapper.State < 27) // reward
        {
            SelectableCard selectedTrial = enumerator.GetFieldValue<object>("<>8__1").GetFieldValue<SelectableCard>("selectedTrialCard");
            if (!selectedTrial)
            {
                List<SelectableCard> cards = instance.selectableCards;
                await Neuro.Cards.UnflipCards(cards, ct);

                List<DeckTrialData> trials = instance.trialChoices.Zip(cards, DeckTrialData.Create).ToList();
                DeckTrialData trial = await Neuro.Loadout.ChooseItem(trials, DecisionStrings.ChooseDeckTrial, ct);
                await NeuroMouse.MoveAndClick(trial.Card, ct);
            }
            // state 22 is trial cards exit (first yield after trial result is available to us)
            await UniTask.WaitUntil(() => wrapper.State is 22, cancellationToken: ct);

            bool passedTrial = enumerator.GetFieldValue<bool>("<trialIsPassed>5__4");
            if (!passedTrial) return;

            await UniTask.WaitUntil(() => wrapper.State is 27, cancellationToken: ct);
        }

        Card reward = await Neuro.Cards.ChooseAddToDeck(instance, ct);
        AutomationManager.Assert(reward, "did not pick deck trial reward");
        await NeuroMouse.MoveAndClick(reward, ct);
    }
}
