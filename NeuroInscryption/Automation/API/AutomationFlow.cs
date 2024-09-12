using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace NeuroInscryption.Automation.API;

[HarmonyPatch]
public abstract class AutomationFlow
{
    static AutomationFlow()
    {
        Flows = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => !t.IsAbstract)
            .Where(t => typeof(AutomationFlow).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<AutomationFlow>()
            .ToList();
    }

    public abstract MethodBase EnableHook { get; }

    public virtual bool AllowInterruption => false;

    public virtual HarmonyPatchType PatchType => HarmonyPatchType.Postfix;

    public virtual bool IsValid(object instanceOfHookedType, object[] parameters) => true;

    public abstract IEnumerator Execute(object instanceOfHookedType, object[] parameters);

    public static IReadOnlyList<AutomationFlow> Flows { get; private set; }
}

public abstract class AutomationFlow<T> : AutomationFlow
{
    protected virtual T GetTarget(object instanceOfHookedType) => (T)instanceOfHookedType;
    protected virtual bool IsValid(T target) => true;
    protected abstract IEnumerator Execute(T target);

    public sealed override bool IsValid(object instanceOfHookedType, object[] parameters) => IsValid(GetTarget(instanceOfHookedType));
    public sealed override IEnumerator Execute(object instanceOfHookedType, object[] parameters) => Execute(GetTarget(instanceOfHookedType));
}
