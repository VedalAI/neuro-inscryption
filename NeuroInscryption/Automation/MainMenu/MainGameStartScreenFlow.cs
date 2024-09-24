using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class MainGameStartScreenFlow : AutomationFlow
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(StartScreenController), nameof(StartScreenController.Start))
        }
    ];

    public override bool CanInterruptItself => true;

    public override UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct)
    {
        // Allow manual control of game for stuff like changing settings
        return UniTask.CompletedTask;
    }
}
