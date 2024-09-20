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

    /// <summary>
    /// A list of Harmony patch hooks that are able to trigger this flow
    /// </summary>
    public abstract FlowHook[] Hooks { get; }

    /// <summary>
    /// A list of <see cref="AutomationFlow"/> types that cannot interrupt this flow
    /// </summary>
    public virtual Type[] FlowsBlockedForInterruption => [];

    /// <summary>
    /// Whether or not this flow can interrupt itself.
    /// <br/>
    /// Setting this to false is equivalent to adding the flow to <see cref="FlowsBlockedForInterruption"/>, but it will not output any logs
    /// </summary>
    public virtual bool CanInterruptItself => true;

    /// <summary>
    /// Whether or not this flow can be interrupted by manual input
    /// </summary>
    public virtual bool AllowManualInterruption => true;

    /// <summary>
    /// How should the flow behave if it's interrupted by manual input
    /// </summary>
    public virtual ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Restart;

    public virtual void ResetState() {}

    public abstract UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct);
}

public abstract class AutomationFlow<T> : AutomationFlow
{
    protected abstract UniTask ExecuteAsync(T target, CancellationToken ct);

    public sealed override UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct) => ExecuteAsync(context.GetInstance<T>()!, ct);
}
