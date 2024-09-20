using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Combat;

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
        CombatState state = CombatState.Calculate();
        PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

        SacrificeAction sacrificeAction = new(card, state);

        string query = string.Format(DecisionStrings.ChooseSacrifices, state.PlayerHand.First(c => c.TargetCard == card).GetDescription());
        Decision decision = new(DecisionIds.CardSacrificeChoice, state, query);
        decision.AddAction(sacrificeAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(sacrificeAction, out IEnumerable<string>? sacrifices))
        {
            IEnumerable<PlayableCard> targets = sacrifices.Select(sacrifice => state.PlayerLanes.Values.First(c => c?.Identifier == sacrifice)!.TargetCard);

            foreach (PlayableCard target in targets)
            {
                await NeuroMouse.MoveToAsync(target, 500, ct);
                await NeuroMouse.ClickAsync(250, ct);
            }
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
