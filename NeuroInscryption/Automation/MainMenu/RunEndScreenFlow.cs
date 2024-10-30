using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

// TODO: flows for reward screens
public sealed class RunEndScreenFlow : HarmonyAutomationFlow<AscensionRunEndScreen>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionRunEndScreen), nameof(AscensionRunEndScreen.Start))
        }
    ];

    public override Type[] FlowsBlockedForInterruption => [typeof(StartScreenFlow)];

    public override ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Cancel;

    protected override async UniTask ExecuteAsync(AscensionRunEndScreen target, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);

        // TODO: Share run results with Neuro
        // TODO: Wait for start new run message from ws

        await UniTask.Delay(4000, cancellationToken: ct);

        AscensionMenuScreenTransition screen = target.GetComponent<AscensionMenuScreenTransition>();

        MainInputInteractable backButton = screen.screenInteractables.Find(i => i.name == "BackButton");
        await NeuroMouse.MoveAndClick(backButton, ct, 500);
    }
}
