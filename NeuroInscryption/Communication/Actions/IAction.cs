using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;

namespace NeuroInscryption.Communication.Actions;

public interface IAction : ISchemaModifier
{
    string ActionName { get; }
}

public interface IAction<TResponse> : IAction
{
    bool Deserialize(Action action, out TResponse? response);
}
