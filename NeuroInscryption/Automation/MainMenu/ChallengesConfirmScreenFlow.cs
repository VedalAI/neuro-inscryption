using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class ChallengesConfirmScreenFlow : HarmonyAutomationFlow<AscensionChallengeConfirmScreen>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionChallengeConfirmScreen), nameof(AscensionChallengeConfirmScreen.OnEnable))
        }
    ];

    protected override async UniTask ExecuteAsync(AscensionChallengeConfirmScreen target, CancellationToken ct)
    {
        await NeuroMouse.MoveAndClick(target.continueButton, ct, 1000);
    }
}
