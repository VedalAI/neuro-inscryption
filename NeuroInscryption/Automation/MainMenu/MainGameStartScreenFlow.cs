using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;

namespace NeuroInscryption.Automation.MainMenu;

internal sealed class MainGameStartScreenFlow : AutomationFlow
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(StartScreenController), nameof(StartScreenController.Start));

    public override bool AllowInterruption => true;

    public override IEnumerator Execute(object _)
    {
        NeuroMouse.SnapOffScreen();
        // Allow manual control of game for stuff like changing settings
        yield break;
    }
}
