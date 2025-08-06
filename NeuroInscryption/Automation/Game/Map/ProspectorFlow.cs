using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;
using NeuroSdk.Actions;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ProspectorFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BoulderChoiceSequencer), nameof(BoulderChoiceSequencer.BoulderChoiceSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(5)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        BoulderChoiceSequencer instance = enumerator.GetThis<BoulderChoiceSequencer>();

        await Wait.CommonFlowDelay(ct);

        BreakBoulderAction breakBoulderAction = new(instance.selectableCards);
        ActionWindow window = ActionWindow.Create(Plugin.PluginObject);
        window.AddAction(breakBoulderAction);
        window.Register();

        await UniTask.WhenAny(breakBoulderAction.ReturningTask);
    }
}
