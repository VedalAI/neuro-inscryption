using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class MainGameStartScreenFlow : AutomationFlow<object>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(StartScreenController), nameof(StartScreenController.Start))
        }
    ];

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(object _)
    {
        NeuroMouse.SnapOffScreen();
        // Allow manual control of game for stuff like changing settings
        yield break;
    }
}
