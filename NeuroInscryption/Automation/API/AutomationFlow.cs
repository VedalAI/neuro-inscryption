using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    public abstract UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct);
}

public abstract class AutomationFlow<T> : AutomationFlow
{
    protected abstract UniTask ExecuteAsync(T target, CancellationToken ct);

    public sealed override UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct) => ExecuteAsync(context.GetInstance<T>()!, ct);
}
