using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class StartScreenFlow : AutomationFlow<AscensionStartScreen>
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
        await NeuroMouse.MoveToAsync(target.RunExists ? target.continueRunText : target.newRunText, ct);
        await UniTask.Delay(500, cancellationToken: ct);
        NeuroMouse.ClickOnce();
    }
}
