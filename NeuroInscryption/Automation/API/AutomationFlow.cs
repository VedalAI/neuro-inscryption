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

public abstract class AutomationFlowAgnostic : AutomationFlow
{
    protected abstract IEnumerator Execute();

    public sealed override bool IsValid(object instanceOfHookedType, object[] parameters) => true;
    public sealed override IEnumerator Execute(object instanceOfHookedType, object[] parameters) => Execute();
}

public abstract class AutomationFlow<T> : AutomationFlow
{
    protected virtual T GetTarget(object instanceOfHookedType) => (T)instanceOfHookedType;
    protected virtual bool IsValid(T target, object[] parameters) => true;
    protected abstract IEnumerator Execute(T target, object[] parameters);

    public sealed override bool IsValid(object instanceOfHookedType, object[] parameters) => IsValid(GetTarget(instanceOfHookedType), parameters);
    public sealed override IEnumerator Execute(object instanceOfHookedType, object[] parameters) => Execute(GetTarget(instanceOfHookedType), parameters);
}

public abstract class AutomationFlowNoParam<TInstance> : AutomationFlow<TInstance>
{
    protected virtual bool IsValid(TInstance target) => true;
    protected abstract IEnumerator Execute(TInstance target);

    protected sealed override bool IsValid(TInstance target, object[] parameters) => IsValid(target);
    protected sealed override IEnumerator Execute(TInstance target, object[] parameters) => Execute(target);
}

public abstract class AutomationFlowParam<TInstance, TParam> : AutomationFlow<TInstance>
{
    protected virtual bool IsValid(TInstance target, TParam parameter) => true;
    protected abstract IEnumerator Execute(TInstance target, TParam parameter);

    protected sealed override bool IsValid(TInstance target, object[] parameters) => IsValid(target, (TParam)parameters[0]);
    protected sealed override IEnumerator Execute(TInstance target, object[] parameters) => Execute(target, (TParam)parameters[0]);
}

public abstract class AutomationFlowParam<TInstance, TParam1, TParam2> : AutomationFlow<TInstance>
{
    protected virtual bool IsValid(TInstance target, TParam1 parameter1, TParam2 parameter2) => true;
    protected abstract IEnumerator Execute(TInstance target, TParam1 parameter1, TParam2 parameter2);

    protected sealed override bool IsValid(TInstance target, object[] parameters) => IsValid(target, (TParam1)parameters[0], (TParam2)parameters[1]);
    protected sealed override IEnumerator Execute(TInstance target, object[] parameters) => Execute(target, (TParam1)parameters[0], (TParam2)parameters[1]);
}

public abstract class AutomationFlowParam<TInstance, TParam1, TParam2, TParam3> : AutomationFlow<TInstance>
{
    protected virtual bool IsValid(TInstance target, TParam1 parameter1, TParam2 parameter2, TParam3 parameter3) => true;
    protected abstract IEnumerator Execute(TInstance target, TParam1 parameter1, TParam2 parameter2, TParam3 parameter3);

    protected sealed override bool IsValid(TInstance target, object[] parameters) => IsValid(target, (TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2]);
    protected sealed override IEnumerator Execute(TInstance target, object[] parameters) => Execute(target, (TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2]);
}
