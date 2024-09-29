using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

public abstract class ManualAutomationFlow<TContext> : IAutomationFlow
{
    public virtual Type[] FlowsBlockedForInterruption => [];

    public virtual bool CanInterruptItself => true;

    public virtual bool AllowManualInterruption { get; protected set; } = true;

    public virtual ManualInterruptionBehaviour ManualInterruptionBehaviour => ManualInterruptionBehaviour.Restart;

    public virtual void ResetState()
    {
        AllowManualInterruption = true;
    }

    UniTask IAutomationFlow.ExecuteAsync(object? context, CancellationToken ct) => ExecuteAsync((TContext) context!, ct);

    protected abstract UniTask ExecuteAsync(TContext context, CancellationToken ct);
}
