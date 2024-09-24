using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ProspectorFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoulderChoiceSequencer), nameof(BoulderChoiceSequencer.BoulderChoiceSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 5
        }
    ];

    public override bool CanInterruptItself => false;

    protected override async UniTask ExecuteAsync(object enumerator, CancellationToken ct)
    {
        BoulderChoiceSequencer instance = enumerator.GetEnumeratorThis<BoulderChoiceSequencer>();

        await UniTask.Delay(500, cancellationToken: ct);

        LoadoutState state = LoadoutState.Calculate();

        BreakBoulderAction breakBoulderAction = new();

        Decision decision = new(DecisionStrings.ChooseBoulder, state);
        decision.AddAction(breakBoulderAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(breakBoulderAction, out int selection)) action.ThrowInvalid();

        await NeuroMouse.MoveToAsync(instance.selectableCards[selection - 1], 200, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
