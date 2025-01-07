using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Combat;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class ChooseSacrificesFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSacrificesForCard))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(1),
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        try
        {
            CombatState state = CombatState.Calculate();
            PlayableCard card = BoardManager.Instance.CurrentSacrificeDemandingCard;

            SacrificeAction sacrificeAction = new(card, state.PlayerLanes);

            string decisionQuery = DecisionStrings.ChooseSacrifices.Format(state.PlayerHand.First(c => c.TargetCard == card).GetDescription());
            IEnumerable<PlayableCardData> sacrifices = await Neuro.Combat.Choose(sacrificeAction, decisionQuery, ct);

            foreach (PlayableCardData sacrifice in sacrifices)
            {
                await NeuroMouse.MoveAndClick(sacrifice.TargetCard, ct, msDelayBeforeClick: 500, msDelayAfterClick: 250);
            }
        }
        catch
        {
            // keeping/handling state mid-sacrifice (e.g. picked 1 of 2 blood) is really not worth the work
            BoardManager.Instance.cancelledPlacementWithInput = true;
            throw;
        }
    }
}
