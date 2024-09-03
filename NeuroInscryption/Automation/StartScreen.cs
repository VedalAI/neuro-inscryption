using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.API.Automation;
using NeuroInscryption.API.Input;
using UnityEngine;

namespace NeuroInscryption.Automation;

internal sealed class StartScreen : AutomationFlow<AscensionStartScreen>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionStartScreen), nameof(AscensionStartScreen.OnEnable));

    protected override IEnumerator Execute(AscensionStartScreen target)
    {
        yield return new WaitForSeconds(1);
        yield return NeuroMouse.MoveTo(target.newRunText);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
