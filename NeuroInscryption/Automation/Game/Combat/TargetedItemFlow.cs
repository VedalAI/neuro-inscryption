using System.Collections;
using System.Collections.Generic;
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
using NeuroInscryption.Utilities.Enumerators;
using ItemData = NeuroInscryption.Communication.Data.Serializable.ItemData;

namespace NeuroInscryption.Automation.Game.Combat;

public class TargetedItemFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(typeof(TargetSlotItem).GetMethod(nameof(TargetSlotItem.ActivateSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(3),
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        TargetSlotItem item = target.GetThis<TargetSlotItem>();
        List<CardSlot> validSlots = item.GetValidTargets();

        ItemData itemData = ItemData.Create(item);
        if (validSlots.Count == 0)
        {
            Plugin.Logger.LogWarning($"No valid targets for {itemData.Identifier} (somehow), attempting to back out of using item");
            await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, ct, 500);
            // CardBattleFlow will interrupt
            return;
        }

        List<SelectableCardData> validTargetCards = validSlots.Select(slot => SelectableCardData.Create(slot.Card, slot.Index)).ToList();
        Description description = DecisionStrings.ChooseItemTarget.Format(itemData.Identifier);

        SelectableCardData card = await Neuro.Combat.ChooseItem(validTargetCards, description, ct, includeDeck: false);

        await NeuroMouse.MoveAndClick(card.TargetCard, ct);
    }
}
