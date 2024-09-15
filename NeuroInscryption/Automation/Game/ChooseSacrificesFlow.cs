using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Automation.Game;

public sealed class ChooseSacrificesFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSacrificesForCard))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 1
        }
    ];

    protected override async UniTask ExecuteAsync(object _, CancellationToken ct)
    {
        GameState.Board state = GameState.Board.Calculate();
        PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

        SacrificeAction sacrificeAction = new(card, state);

        string query = string.Format(DecisionStrings.ChooseSacrificesDecision, state.PlayerHand.Find(c => c.TargetCard == card).GetDescription());
        Decision decision = new("card_sacrifice_choice", state, query);
        decision.AddAction(sacrificeAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(sacrificeAction, out IEnumerable<string>? sacrifices))
        {
            IEnumerable<PlayableCard> targets = sacrifices.Select(sacrifice => state.PlayerLanes.Values.First(c => c?.Name == sacrifice)!.TargetCard!);

            foreach (PlayableCard target in targets)
            {
                await NeuroMouse.MoveToAsync(target, ct);
                await UniTask.Delay(500, cancellationToken: ct);
                NeuroMouse.ClickOnce();
                await UniTask.Delay(250, cancellationToken: ct);
            }
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
