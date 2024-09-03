using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.API.Automation;
using NeuroInscryption.API.Input;

namespace NeuroInscryption.Automation;

internal sealed class MainGameStartScreen : AutomationFlow
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(StartScreenController), nameof(StartScreenController.Start));

    public override IEnumerator Execute(object _)
    {
        SimulatedMouse.SnapOffScreen();
        yield break;
    }
}
