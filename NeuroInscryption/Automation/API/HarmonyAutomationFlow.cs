using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

extern alias JetBrains;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class HarmonyAutomationFlow : AutomationFlow<HarmonyContext>
{
    /// <summary>
    /// A list of Harmony patch hooks that are able to trigger this flow
    /// </summary>
    public abstract FlowHook[] Hooks { get; }

    protected override bool ShouldAbort(HarmonyContext context)
    {
        object? instance = context.GetInstance();
        if (instance == null || instance is UnityEngine.Object unityObject && unityObject == null)
        {
            Plugin.Logger.LogWarning($"Flow {FlowName} was cancelled because the target object was destroyed.");
            return true;
        }

        if (instance is UnityEngine.GameObject { activeInHierarchy: false })
        {
            Plugin.Logger.LogWarning($"Flow {FlowName} was cancelled because the target object was inactive.");
            return true;
        }

        return false;
    }
}

// TODO: maybe we can remove this
public abstract class HarmonyAutomationFlow<T> : HarmonyAutomationFlow
{
    protected abstract UniTask ExecuteAsync(T target, CancellationToken ct);

    protected sealed override UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct) => ExecuteAsync(context.GetInstance<T>()!, ct);
}
