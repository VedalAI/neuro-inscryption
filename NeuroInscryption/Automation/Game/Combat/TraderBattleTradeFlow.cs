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

public class TraderBattleTradeFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(typeof(TradeCardsForPelts).GetMethod(nameof(TradeCardsForPelts.TradePhase))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(11)
        }
    ];
    
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(List<PlayableCard> cards, int amount) : ICombatContext
    {
        public readonly IReadOnlyList<SelectableCardData> CardOptions = cards.Select(SelectableCardData.Create).ToList();
        public readonly int NumberOfCardsToBuy = amount;
        public bool IncludeDeck => true;
    }

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct);

        int numPelts = PlayerHand.Instance.CardsInHand.Count(c => c.Info.HasTrait(Trait.Pelt));
        if (numPelts == 0)
        {
            Plugin.Logger.LogMessage("No pelts to trade - flow restarted? Trading phase should end on its own soon, otherwise o7");
            return;
        }
        
        List<PlayableCard> cards = BoardManager.Instance.opponentSlots
            .Select(slot => slot.Card)
            .Concat(TurnManager.Instance.Opponent.Queue)
            .WhereNotNull()
            .ToList();

        CombatState state = CombatState.Calculate();
        Context context = new(cards, numPelts);
        state.SetContext(context);

        ChooseMultipleAction<SelectableCardData> chooseAction = new(context.CardOptions, context.NumberOfCardsToBuy);

        Decision decision = new(DecisionStrings.ChooseTraderCardsBattle.Format(context.NumberOfCardsToBuy), state);
        decision.AddAction(chooseAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(chooseAction, out IEnumerable<SelectableCardData>? chosenCards)) action.ThrowInvalid();

        foreach (SelectableCardData card in chosenCards)
        {
            await NeuroMouse.MoveToAsync(card.TargetCard, 500, ct);
            AllowManualInterruption = false;
            await NeuroMouse.ClickAsync(250, ct);
        }

        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, 250, ct);
    }
}