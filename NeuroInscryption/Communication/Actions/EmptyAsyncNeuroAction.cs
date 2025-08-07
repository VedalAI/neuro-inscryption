using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;

namespace NeuroInscryption.Communication.Actions;

public abstract class EmptyAsyncNeuroAction : AsyncNeuroAction
{
    protected sealed override JsonSchema Schema => QJS.ConstNull;
    protected sealed override ExecutionResult Validate(ActionJData actionData) => ExecutionResult.Success();
}