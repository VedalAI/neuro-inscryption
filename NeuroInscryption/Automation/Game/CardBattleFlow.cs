using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Automation.Game;

public class CardBattleFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(TurnManager), nameof(TurnManager.PlayerTurn))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 8,
        },
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(PlayerHand), nameof(PlayerHand.SelectSlotForCard))),
            IsValid = context => context.GetResult<bool>() == false,
        }
    ];

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        if (enumerator.GetType().Name.Contains(nameof(PlayerHand.SelectSlotForCard)))
        {
            await UniTask.Delay(1000, cancellationToken: ct); // time it takes for the card to go back in the hand
        }

        ViewManager.Instance.SwitchToView(BoardManager.Instance.DefaultView);

        GameState.Board state = GameState.Board.Calculate();

        PlayCardAction playCardAction = new(state);

        Decision decision = new("card_battle_turn", state, DecisionStrings.CardBattleDecision);
        decision.AddAction(playCardAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(playCardAction, out string? card))
        {
            PlayableCard target = state.PlayerHand.Find(c => c.Name == card).TargetCard!;

            await NeuroMouse.MoveToAsync(target, ct);
            await UniTask.Delay(500, cancellationToken: ct);
            NeuroMouse.ClickOnce();
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
