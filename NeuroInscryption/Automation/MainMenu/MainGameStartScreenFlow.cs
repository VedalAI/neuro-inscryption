using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class MainGameStartScreenFlow : AutomationFlowAgnostic
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(StartScreenController), nameof(StartScreenController.Start));

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute()
    {
        NeuroMouse.SnapOffScreen();
        // Allow manual control of game for stuff like changing settings
        yield break;
    }
}
