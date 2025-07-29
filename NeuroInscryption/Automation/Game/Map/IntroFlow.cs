using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;
using UnityEngine;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class IntroFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(RunIntroSequencer), nameof(RunIntroSequencer.RunIntroSequence))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(18)
        }
    ];

    public override Type[] FlowsBlockedForInterruption => [typeof(ChoosePathFlow)];
    public override bool CanInterruptItself => false;

    public override ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Cancel;

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        OilPaintingPuzzle puzzle = GameObject.FindObjectOfType<OilPaintingPuzzle>();

        await Wait.CommonFlowDelay(ct);

        // Stop inspecting deck
        ViewManager.Instance.SwitchToView(View.MapDefault);

        return; // todo: we're not sending state of oil painting puzzle yet so this is pointless to check
        
        if (puzzle.state.puzzleSolved) return;

        AutomationManager.Assert(ViewManager.Instance.Is(ViewLockState.Unlocked));
        await UniTask.Delay(1000, cancellationToken: ct);
        AllowManualInterruption = false;
        // Stand up
        await Wait.ViewUnlocked(ct);
        // i'm so done dude
        for (int tries = 0; tries < 3; tries++)
        {
            await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, ct, 500);
            await Wait.CameraStable(ct);
            if (ViewManager.Instance.Is(View.FirstPerson)) break;
        }

        if (!ViewManager.Instance.Is(View.FirstPerson))
        {
            // whatever
            Plugin.Logger.LogWarning("No painting for you today");
            return;
        }

        // Move to painting
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, ct);

        // Inspect painting
        await NeuroMouse.MoveAndClick(puzzle.zoomInteractable, ct, mouseSpeedMultiplier: 10);
        // todo: send context here
        await UniTask.Delay(1750, cancellationToken: ct); // explicit "looking" wait

        // Return to table
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, ct);
        await Wait.View(View.FirstPerson, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, ct);

        await Wait.CameraStable(ct); // just in case
    }
}
