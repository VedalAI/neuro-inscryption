using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
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

    public override void ResetState()
    {
        AllowManualInterruption = true;
    }

    protected override async UniTask ExecuteAsync(object _, CancellationToken ct)
    {
        CombatState state = CombatState.Calculate();
        PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

        SacrificeAction sacrificeAction = new(card, state.PlayerLanes);

        Description decisionString = DecisionStrings.ChooseSacrifices.Format(state.PlayerHand.First(c => c.TargetCard == card).GetDescription());
        Decision decision = new(decisionString, state);
        decision.AddAction(sacrificeAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(sacrificeAction, out IEnumerable<PlayableCardData>? sacrifices)) action.ThrowInvalid();

        foreach (PlayableCardData sacrifice in sacrifices)
        {
            await NeuroMouse.MoveToAsync(sacrifice.TargetCard, 500, ct);
            AllowManualInterruption = false;
            await NeuroMouse.ClickAsync(250, ct);
        }
    }
}
