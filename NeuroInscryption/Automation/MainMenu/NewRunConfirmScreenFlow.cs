using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class NewRunConfirmScreenFlow : HarmonyAutomationFlow<AscensionMenuScreenTransition>
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
        await NeuroMouse.MoveToAsync(target.screenInteractables.First(i => i.name == "ContinueButton"), 500, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}
