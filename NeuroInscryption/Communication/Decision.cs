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

    private sealed class DecisionAction(Decision decision, ActionBase ourAction) : NeuroAction
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
            catch
            {
                return ExecutionResult.Failure("Something went wrong."); // placeholder
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

}
