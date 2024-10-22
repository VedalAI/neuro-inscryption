using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
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

        await UniTask.Delay(500, cancellationToken: ct);

        // fun exercise: random order for picking sac/host
        SelectableCard sacrifice = await PickSacrifice(instance, ct);
        Debug.Assert(sacrifice is { }, "did not pick sacrifice");

        await UniTask.Delay(300, cancellationToken: ct);

        SelectableCard host = await PickHost(instance, ct);
        Debug.Assert(host is { }, "did not pick host");

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }

    private async UniTask<SelectableCard> PickSacrifice(CardMergeSequencer instance, CancellationToken ct)
    {
        SelectableCard alreadySelected = instance.sacrificeSlot.Card;
        if (alreadySelected)
        {
            Plugin.Logger.LogMessage($"Already selected sacrifice {alreadySelected}");
            return alreadySelected;
        }

        await NeuroMouse.MoveToAsync(instance.sacrificeSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        List<SelectableCard> sacrificable = instance.sacrificeSlot.cardSelector.displayedCards
            .Where(card => instance.GetValidCardsForHost(card.Info).Any())
            .ToList();

        Debug.Assert(sacrificable.Count > 0, "No sacrificable cards found"); // you can't cancel the stones iirc

        SelectableCard card = await Neuro.Cards.ChooseCardLoadout(sacrificable, DecisionStrings.Sacrifice, ct);
        await NeuroMouse.MoveToAsync(card, 200, ct);
        await NeuroMouse.ClickAsync(ct);

        return card;
    }

    private async UniTask<SelectableCard> PickHost(CardMergeSequencer instance, CancellationToken ct)
    {
        SelectableCard alreadySelected = instance.hostSlot.Card;
        if (alreadySelected)
        {
            Plugin.Logger.LogMessage($"Already selected host {alreadySelected}");
            return alreadySelected;
        }

        SelectableCard sac = instance.sacrificeSlot.Card;
        Debug.Assert((bool)sac, "Should have picked a sacrifice by this point");
        // alternatively,
        //if (!sac) sac = await PickSacrifice(instance, ct);

        await NeuroMouse.MoveToAsync(instance.hostSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        List<SelectableCard> hosts = instance.hostSlot.cardSelector.displayedCards;
        Debug.Assert(hosts.Count > 0, "No host cards found");

        SelectableCard card = await Neuro.Cards.ChooseCardLoadout(hosts, DecisionStrings.SacrificeEmpower.Format(CardData.Create(sac.Info).GetDescription()), ct);
        await NeuroMouse.MoveToAsync(card, 200, ct);
        await NeuroMouse.ClickAsync(ct);

        return card;
    }
}
