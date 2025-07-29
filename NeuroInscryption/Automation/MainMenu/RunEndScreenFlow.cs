using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroSdk.Messages.Outgoing;

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

    public override ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Cancel;

    protected override async UniTask ExecuteAsync(AscensionRunEndScreen target, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);

        Context.Send(GenericStrings.EndRun.Format(target.isVictory ? GenericStrings.EndRunVictory : GenericStrings.EndRunDefeat));
        
        await UniTask.Delay(4000, cancellationToken: ct);

        AscensionMenuScreenTransition screen = target.GetComponent<AscensionMenuScreenTransition>();

        MainInputInteractable backButton = screen.screenInteractables.Find(i => i.name == "BackButton");
        await NeuroMouse.MoveAndClick(backButton, ct, 500);
    }
}
