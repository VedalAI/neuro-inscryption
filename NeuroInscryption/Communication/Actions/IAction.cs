using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions;

public interface IAction
{
    bool IsApplicable { get; }
    string Name { get; }
    JsonSchema? GetDataSchema();
}

public interface IAction<T> : IAction
{
    bool TryDeserialize(JToken? data, out T? response);
}
