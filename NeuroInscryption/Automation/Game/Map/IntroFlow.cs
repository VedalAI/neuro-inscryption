using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
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

        // Stop inspecting deck
        ViewManager.Instance.SwitchToView(View.MapDefault);
        if (puzzle.state.puzzleSolved) return;

        await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);
        await UniTask.Delay(1000, cancellationToken: ct);
        AllowManualInterruption = false;
        // Stand up
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, 1000, ct);
        await UniTask.WaitUntil(() => ViewManager.Instance.CurrentView == View.FirstPerson, cancellationToken: ct);
        await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);

        // Move to painting
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, 250, ct);

        // Inspect painting
        await NeuroMouse.MoveToAsync(puzzle.zoomInteractable, ct, mouseSpeedMultiplier: 10);
        await NeuroMouse.ClickAsync(2500, ct);

        // Return to table
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, 250, ct);
        NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp);
    }
}
