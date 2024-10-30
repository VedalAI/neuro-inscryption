using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

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

        int selection = await Neuro.Loadout.Choose(new BreakBoulderAction(), DecisionStrings.ChooseBoulder, ct);
        AutomationManager.Assert(selection is >= 1 and <= 3, "picked boulder out of range");
#if !DEBUG // failsafe
        if (selection is <1 or >3)
        {
            Plugin.Logger.LogError($"Boulder out of range - selected {selection}, should be between 1 and 3");
            selection = UnityEngine.Random.RandomRangeInt(1, 3);
        }
#endif

        SelectableCard boulder = instance.selectableCards[selection - 1];
        await NeuroMouse.MoveAndClick(boulder, ct);
    }
}
