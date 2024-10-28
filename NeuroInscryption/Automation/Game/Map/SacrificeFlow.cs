using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class SacrificeFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardMergeSequencer), nameof(CardMergeSequencer.MergeSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(12)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        CardMergeSequencer instance = enumerator.GetThis<CardMergeSequencer>();

        await Wait.CommonFlowDelay(ct);

        if (!ViewManager.Instance.Is(View.CardMergeConfirm))
        {
            // there's an unlikely edge case here
            // if host slot is filled (through an error-restart or manual intervention), sacrifice will be picked blindly
            await PickSacrifice(instance, ct);
            await PickHost(instance, ct);
            await Wait.View(View.CardMergeConfirm, ct);
        }

        await NeuroMouse.MoveAndClick(instance.confirmStone, ct);
    }

    private static async UniTask<Card> PickSacrifice(CardMergeSequencer instance, CancellationToken ct)
    {
        SelectableCard alreadySelected = instance.sacrificeSlot.Card;
        if (alreadySelected)
        {
            Plugin.Logger.LogMessage($"Already selected sacrifice {alreadySelected}");
            return alreadySelected;
        }

        Card sacrifice = await Neuro.Cards.SelectCardFromDeck(instance.sacrificeSlot, DecisionStrings.Sacrifice, ct, validSacrificeFilter);
        AutomationManager.Assert(sacrifice, "did not pick sacrifice");
        return sacrifice;

        bool validSacrificeFilter(SelectableCard c) => instance.GetValidCardsForSacrifice(c.Info).Any();
    }

    private static async UniTask<Card> PickHost(CardMergeSequencer instance, CancellationToken ct)
    {
        SelectableCard alreadySelected = instance.hostSlot.Card;
        if (alreadySelected)
        {
            Plugin.Logger.LogMessage($"Already selected host {alreadySelected}");
            return alreadySelected;
        }

        Card sac = instance.sacrificeSlot.Card;
        if (!sac) sac = await PickSacrifice(instance, ct);
        // alternatively,
        // AutomationManager.Assert(sac, "Should have picked a sacrifice by this point");

        Description desc = DecisionStrings.SacrificeEmpower.Format(CardData.Create(sac.Info).GetDescription());
        Card host = await Neuro.Cards.SelectCardFromDeck(instance.hostSlot, desc, ct);
        AutomationManager.Assert(host, "did not pick host");
        return host;
    }
}
