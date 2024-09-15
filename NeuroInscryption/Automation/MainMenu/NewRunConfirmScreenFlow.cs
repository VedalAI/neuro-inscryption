using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class NewRunConfirmScreenFlow : AutomationFlow<AscensionMenuScreenTransition>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionMenuScreenTransition), nameof(AscensionMenuScreenTransition.OnEnable)),
            IsValid = context => context.GetInstance<AscensionMenuScreenTransition>()!.name == "AscensionNewRunConfirmScreen"
        }
    ];

    protected override async UniTask ExecuteAsync(AscensionMenuScreenTransition target, CancellationToken ct)
    {
        await NeuroMouse.MoveToAsync(target.screenInteractables[1], ct);
        await UniTask.Delay(500, cancellationToken: ct);
        NeuroMouse.ClickOnce();
    }
}
