using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.State;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Utilities;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication;

public record struct DecisionResult(string PickedAction, JToken? Data);

public sealed class Decision(string query, IGameState state)
{
    private readonly ActionWindow _actionWindow = ActionWindow.Create(Plugin.PluginObject); // scuff?

    public DecisionResult? Result { get; private set; }

    private UniTaskCompletionSource _tcs = new();
    public float Timeout { get; init; } = 1; // short for randy; todo
    public bool IsEphemeral { get; init; } // todo

    public void AddAction(ActionBase action)
    {
        _actionWindow.AddAction(new DecisionAction(this, action));
    }

    public void AddAction<T>(ActionBase<T> action)
    {
        _actionWindow.AddAction(new DecisionAction<T>(this, action));
    }

    public UniTask Send(CancellationToken ct)
    {
        // todo: better state representation
        if (Timeout >= 0) _actionWindow.SetForce(Timeout, () => query, () => Jason.Serialize(state), IsEphemeral);
        _actionWindow.SetEnd(() => ct.IsCancellationRequested);
        _actionWindow.Register();
        return _tcs.Task;
    }

    [DoesNotReturn]
    public void ThrowInvalid()
    {
        AutomationManager.Assert(Result.HasValue, "ThrowInvalid without Result");
        throw new InvalidOperationException($"Received action '{Result.Value.PickedAction}' which did not match possible expected actions.");
    }

    private class DecisionAction(Decision decision, ActionBase ourAction) : NeuroAction
    {
        public override string Name => ourAction.Name;
        protected override string Description => ourAction.Description;
        public override bool CanAddToActionWindow(ActionWindow window) => ourAction.IsApplicable;

        protected override JsonSchema? Schema => ourAction.GetDataSchema();
        protected override ExecutionResult Validate(ActionJData actionData)
        {
            try
            {
                decision.Result = new(Name, actionData.Data);
                return ExecutionResult.Success();
            }
            catch (Exception e)
            {
                return ExecutionResult.Failure("Something went wrong." + e); // placeholder
            }
        }

        protected override UniTask ExecuteAsync()
        {
            try
            {
                // any continuations run here
                decision._tcs.TrySetResult();
            }
            finally
            {
                decision._tcs = new UniTaskCompletionSource();
            }
            return UniTask.CompletedTask;
        }
    }

    private sealed class DecisionAction<T>(Decision decision, ActionBase<T> ourAction) : DecisionAction(decision, ourAction)
    {
        private readonly Decision _decision = decision;
        protected override ExecutionResult Validate(ActionJData actionData)
        {
            try
            {
                _decision.Result = new(Name, actionData.Data);
                // ISSUE: the data is deserialized again later in the action itself (ActionBase<T>.TryDeserialize)
                // as it is, it's just a small perf hit - would need a big refactor to actually fix
                if (ourAction.Matches(_decision, out _))
                    return ExecutionResult.Success();
                return ExecutionResult.Failure("Failed to deserialize action data. Please check that the data conforms to the given schema.");
            }
            catch (Exception e)
            {
                return ExecutionResult.Failure("Something went wrong: " + e); // placeholder
            }
        }
    }
}
