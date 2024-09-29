using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class BoneAltarFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardRemoveSequencer), nameof(CardRemoveSequencer.RemoveSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 7
        }
    ];

    protected override async UniTask ExecuteAsync(object target, CancellationToken ct)
    {
        CardRemoveSequencer instance = target.GetEnumeratorThis<CardRemoveSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        await NeuroMouse.MoveToAsync(instance.sacrificeSlot, 200, ct);
        await NeuroMouse.ClickAsync(400, ct);

        LoadoutState state = LoadoutState.Calculate();

        IEnumerable<SelectableCardData> cards = instance.sacrificeSlot.cardSelector.displayedCards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        ChooseAction<SelectableCardData> chooseCardAction = new(cards, true);

        Decision decision = new(DecisionStrings.BoneAltarChooseCard, state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(selected.TargetCard, 200, ct);
        await NeuroMouse.ClickAsync(1000, ct);

        await NeuroMouse.MoveToAsync(instance.confirmStone, 250, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
