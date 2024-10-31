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
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class MagpieDrawFlow : HarmonyAutomationFlow<IEnumerator>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class MagpieDrawContext(IEnumerable<SelectableCard> cards, IGameState state) : ICombatContext
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
        MagpieDrawContext context = new(BoardManager.Instance.CardSelector.displayedCards, state);

        ChooseAction<SelectableCardData> chooseCardAction = new(context.RemainingDeckToDrawFrom, ActionStrings.DrawCard);

        SelectableCardData selected = await Neuro.Common.Choose(chooseCardAction, DecisionStrings.MagpieDrawCard, state, ct, context);

        await NeuroMouse.MoveAndClick(selected.TargetCard, ct);
    }
}
