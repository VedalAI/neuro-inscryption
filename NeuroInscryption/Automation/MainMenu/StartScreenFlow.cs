using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class StartScreenFlow : HarmonyAutomationFlow<AscensionStartScreen>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionStartScreen), nameof(AscensionStartScreen.OnEnable))
        }
    ];

    protected override async UniTask ExecuteAsync(AscensionStartScreen target, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);
        AscensionMenuInteractable menuItem = target.RunExists ? target.continueRunText : target.newRunText;
        await NeuroMouse.MoveAndClick(menuItem, ct, 500);
    }
}
