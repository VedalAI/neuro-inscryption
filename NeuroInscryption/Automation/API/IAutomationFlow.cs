using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

public interface IAutomationFlow
{
    Type[] FlowsBlockedForInterruption { get; }
    bool CanInterruptItself { get; }
    bool AllowManualInterruption { get; }
    ManualInterruptionBehaviour ManualInterruptionBehaviour { get; }

    void ResetState();
    UniTask ExecuteAsync(object? context, CancellationToken ct);
}
