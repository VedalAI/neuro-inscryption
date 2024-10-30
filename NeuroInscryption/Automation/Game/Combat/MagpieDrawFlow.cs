using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class MagpieDrawFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(IEnumerable<SelectableCard> cards, IGameState state) : ICombatContext
    {
        public readonly IReadOnlyList<SelectableCardData> RemainingDeckToDrawFrom = cards.Select((c, i) => SelectableCardData.Create(c, i, state)).ToList();

        bool ICombatContext.IncludeDeck => false;
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(Deck), nameof(Deck.ChooseCard))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(1)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        CombatState state = CombatState.Calculate();
        Context context = new(BoardManager.Instance.CardSelector.displayedCards, state);
        state.SetContext(context);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.RemainingDeckToDrawFrom, ActionStrings.DrawCard);

        Decision decision = new(DecisionStrings.MagpieDrawCard, state);
        decision.AddAction(chooseCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseCardAction, out SelectableCardData? selected)) action.ThrowInvalid();

        await NeuroMouse.MoveAndClick(selected.TargetCard, ct);
    }
}
