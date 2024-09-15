using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace NeuroInscryption.Automation.API;

[HarmonyPatch]
public abstract class AutomationFlow
{
    public static IReadOnlyList<AutomationFlow> Flows { get; private set; }

    static AutomationFlow()
    {
        Flows = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => !t.IsAbstract)
            .Where(t => typeof(AutomationFlow).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<AutomationFlow>()
            .ToList();
    }

    public abstract FlowHook[] Hooks { get; }

    public abstract IEnumerator Execute(HarmonyContext context);
}

public abstract class AutomationFlow<T> : AutomationFlow
{
    protected virtual T GetTarget(object instanceOfHookedType) => (T)instanceOfHookedType;
    protected abstract IEnumerator Execute(T target);

    public sealed override IEnumerator Execute(HarmonyContext context) => Execute(GetTarget(context.GetInstance<T>()!));
}
