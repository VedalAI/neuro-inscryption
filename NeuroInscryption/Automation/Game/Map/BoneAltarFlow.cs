using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class BoneAltarFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardRemoveSequencer), nameof(CardRemoveSequencer.RemoveSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(7),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        CardRemoveSequencer instance = target.GetThis<CardRemoveSequencer>();

        await Wait.CameraStable(ct);
        await UniTask.Delay(300, cancellationToken: ct); // todo: centralised source for "human" delay

        if (!ViewManager.Instance.Is(View.CardMergeConfirm))
        {
            if (ViewManager.Instance.Is(View.CardMergeSlots))
            {
                await NeuroMouse.MoveAndClick(instance.sacrificeSlot, ct,
                    delayAfterClick: Wait.CardPileFullSpawnTimeMs);
            }

            await Wait.View(View.DeckSelection, ct);

            List<SelectableCard> cards = instance.sacrificeSlot.cardSelector.displayedCards;
            Card card = await Neuro.Cards.ChooseCardLoadout(cards, DecisionStrings.BoneAltarChooseCard, ct);
            AutomationManager.Assert(card, "did not pick bone altar card");

            await NeuroMouse.MoveAndClick(card, ct);

            await Wait.View(View.CardMergeConfirm, ct);
        }

        if (!instance.confirmStone.SelectionConfirmed)
        {
            await NeuroMouse.MoveAndClick(instance.confirmStone, ct);
            await UniTask.WaitUntil(() => !instance.sacrificeSlot.Card, cancellationToken: ct);
        }

        // slightly cursed but the boon card isn't actually placed on the slot, it's just moved to the same position
        List<InteractableBase> interactables = instance.gamepadGrid.Rows[0].interactables;
        await UniTask.WaitUntil(() => interactables.Count == 2 && interactables[1].Enabled, cancellationToken: ct);
        await NeuroMouse.MoveAndClick(interactables[1], ct);
    }
}
