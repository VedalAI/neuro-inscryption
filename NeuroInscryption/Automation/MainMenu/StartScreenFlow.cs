using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using UnityEngine;

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

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(AscensionStartScreen target)
    {
        yield return new WaitForSeconds(1);
        yield return NeuroMouse.MoveTo(target.RunExists ? target.continueRunText : target.newRunText);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
