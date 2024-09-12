using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using UnityEngine;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class NewRunConfirmScreenFlow : AutomationFlowNoParam<AscensionMenuScreenTransition>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionMenuScreenTransition), nameof(AscensionMenuScreenTransition.OnEnable));

    public override bool AllowInterruption => true;

    protected override bool IsValid(AscensionMenuScreenTransition target)
    {
        return target.name == "AscensionNewRunConfirmScreen";
    }

    protected override IEnumerator Execute(AscensionMenuScreenTransition target)
    {
        yield return NeuroMouse.MoveTo(target.screenInteractables[1]);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
