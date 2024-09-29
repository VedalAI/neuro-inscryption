using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

public abstract class AutomationFlow<TContext> : IAutomationFlow
{
    public string FlowName => GetType().Name;

    /// <summary>
    /// A list of <see cref="HarmonyAutomationFlow"/> types that cannot interrupt this flow
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
    public virtual bool AllowManualInterruption { get; protected set; } = true;

    /// <summary>
    /// How should the flow behave if it's interrupted by manual input
    /// </summary>
    public virtual ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Restart;

    protected virtual bool ShouldAbort(TContext context) => false;

    public virtual void ResetState()
    {
        AllowManualInterruption = true;
    }

    protected abstract UniTask ExecuteAsync(TContext context, CancellationToken ct);

    bool IAutomationFlow.ShouldAbort(object? context) => ShouldAbort((TContext) context!);
    UniTask IAutomationFlow.ExecuteAsync(object? context, CancellationToken ct) => ExecuteAsync((TContext) context!, ct);
}
