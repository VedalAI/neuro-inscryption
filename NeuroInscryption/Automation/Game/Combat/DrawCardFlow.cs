using System.Collections;
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
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class DrawCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardDrawPiles3D), nameof(CardDrawPiles3D.ChooseDraw))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(2)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        bool drawFromSideDeck = await DoWeDrawFromSideDeck(ct);

        CardPile target = !drawFromSideDeck ? CardDrawPiles3D.Instance.Pile : CardDrawPiles3D.Instance.SidePile;
        await NeuroMouse.MoveAndClick(target, ct, msDelayBeforeClick: 500);
    }

    private static async UniTask<bool> DoWeDrawFromSideDeck(CancellationToken ct = default)
    {
        CombatState state = CombatState.Calculate();

        bool mainDeckEmpty = state.RemainingDeck?.Count == 0;
        bool sideDeckEmpty = state.RemainingSquirrels == "0";

        if (mainDeckEmpty)
        {
            Plugin.Logger.LogMessage($"Main deck empty, force draw from side");
            return true;
        }
        if (sideDeckEmpty)
        {
            Plugin.Logger.LogMessage($"Side deck empty, force draw from main");
            return false;
        }
        DrawCardAction drawCardAction = new(mainDeckEmpty, sideDeckEmpty);

        Decision decision = new(DecisionStrings.DrawCard, state);
        decision.AddAction(drawCardAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(drawCardAction, out bool drawFromSideDeck)) action.ThrowInvalid();

        return drawFromSideDeck;
    }
}
