using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

public sealed class FlowRunner
{
    private IAutomationFlow? _currentFlow;
    private object _currentContext = null!;
    private CancellationTokenSource? _cts;

    public string CurrentFlowName => _currentFlow?.FlowName ?? "<none>";

    public bool AllowManualInterruption => _currentFlow?.AllowManualInterruption ?? true;

    public void StartFlow(IAutomationFlow flow, object context)
    {
        QueueFlow(flow, context);
        RestartLastFlow();
    }

    public void CancelCurrentFlowForManualInput()
    {
        _cts?.Cancel();

        if (_currentFlow?.ManualInterruptionBehaviour == ManualInterruptionBehaviour.Cancel)
        {
            Plugin.Logger.LogInfo($"Flow {CurrentFlowName} was cancelled for manual input and will not restart.");
            _currentFlow = null;
        }
    }

    public void CancelCurrentFlowForcefully()
    {
        Plugin.Logger.LogInfo($"Forcefully cancelled current flow {CurrentFlowName}.");
        _cts?.Cancel();
        _currentFlow = null;
    }

    public void QueueFlow(IAutomationFlow flow, object context)
    {
        if (_currentFlow != null)
        {
            if (_currentFlow.FlowsBlockedForInterruption.Contains(flow.GetType()))
            {
                Plugin.Logger.LogMessage($"Preventing changing flow from {_currentFlow.FlowName} to {flow.GetType().Name}.");
                return;
            }

            if (_currentFlow.GetType() == flow.GetType() && !_currentFlow.CanInterruptItself)
            {
                return;
            }
        }

        Plugin.Logger.LogMessage($"Changing flow from {CurrentFlowName} to {flow.GetType().Name}.");

        _cts?.Cancel();

        flow.ResetState();
        _currentFlow = flow;
        _currentContext = context;
    }

    public void RestartLastFlow()
    {
        if (_currentFlow == null) return;

        if (_cts?.IsCancellationRequested == false)
        {
            if (_currentFlow.FlowsBlockedForInterruption.Contains(_currentFlow.GetType()))
            {
                Plugin.Logger.LogInfo($"Restarting flow {_currentFlow.FlowName} was blocked.");
                return;
            }

            if (!_currentFlow.CanInterruptItself)
            {
                return;
            }
        }

        Plugin.Logger.LogMessage($"Starting flow {_currentFlow.FlowName}.");

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();
        ExecuteFlowAsync(_currentFlow).Forget();
    }

    private async UniTaskVoid ExecuteFlowAsync(IAutomationFlow flowToExecute)
    {
        try
        {
            if (flowToExecute.ShouldAbort(_currentContext)) return;

            flowToExecute.ResetState();
            await flowToExecute.ExecuteAsync(_currentContext, _cts!.Token);

            if (flowToExecute != _currentFlow)
            {
                Plugin.Logger.LogMessage($"Flow {flowToExecute.FlowName} was cancelled too late and still finished.");
                return;
            }

            Plugin.Logger.LogMessage($"Flow {flowToExecute.FlowName} completed.");
            _currentFlow = null;
        }
        catch (OperationCanceledException)
        {
            Plugin.Logger.LogMessage($"Flow {flowToExecute.FlowName} was cancelled.");
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Caught exception while executing flow {flowToExecute.FlowName}. Flow will be restarted");
            Plugin.Logger.LogError(e);
            RestartLastFlow();
        }
    }
}
