using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using UnityEngine;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class NewRunConfirmScreenFlow : AutomationFlow<AscensionMenuScreenTransition>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionMenuScreenTransition), nameof(AscensionMenuScreenTransition.OnEnable)),
            IsValid = context => context.GetInstance<AscensionMenuScreenTransition>().name == "AscensionNewRunConfirmScreen"
        }
    ];

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(AscensionMenuScreenTransition target)
    {
        yield return NeuroMouse.MoveTo(target.screenInteractables[1]);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
