using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;

namespace NeuroInscryption.Communication.Actions;

public interface IAction<TResponse> : ISchemaModifier
{
    string ActionName { get; }
    bool Deserialize(Action action, out TResponse? response);
}
