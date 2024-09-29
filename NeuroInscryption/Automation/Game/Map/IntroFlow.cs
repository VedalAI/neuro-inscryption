using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class IntroFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(RunIntroSequencer), nameof(RunIntroSequencer.RunIntroSequence))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 18
        }
    ];

    public override Type[] FlowsBlockedForInterruption => [typeof(ChoosePathFlow)];
    public override bool CanInterruptItself => false;

    public override ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Cancel;

    protected override async UniTask ExecuteAsync(object target, CancellationToken ct)
    {
        OilPaintingPuzzle puzzle = GameObject.FindObjectOfType<OilPaintingPuzzle>();

        // Stop inspecting deck
        ViewManager.Instance.SwitchToView(View.MapDefault);
        if (puzzle.state.puzzleSolved) return;

        // Move to painting
        await UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);
        await UniTask.Delay(1000, cancellationToken: ct);
        AllowManualInterruption = false;
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookDown, 1000, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.DirUp, 250, ct);
        await NeuroKeyboard.Press(NeuroKeyboard.Button.LookRight, 250, ct);

        // Inspect paiting
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
