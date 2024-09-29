using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithInheritors)]
public interface IAutomationFlow
{
    string FlowName { get; }
    Type[] FlowsBlockedForInterruption { get; }
    bool CanInterruptItself { get; }
    bool AllowManualInterruption { get; }
    ManualInterruptionBehaviour ManualInterruptionBehaviour { get; }

    bool ShouldAbort(object context);
    void ResetState();
    UniTask ExecuteAsync(object context, CancellationToken ct);
}
