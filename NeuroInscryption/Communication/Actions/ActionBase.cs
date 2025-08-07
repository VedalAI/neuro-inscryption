using System.Diagnostics.CodeAnalysis;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions;

public abstract class ActionBase(string name, string description = "")
{
    /// <summary>If <see langword="false"/>, will not be registered as a possible action in the <see cref="NeuroSdk.Actions.ActionWindow"/>.</summary>
    public virtual bool IsApplicable => true;
    public virtual string Name => name;
    public virtual string Description => description;
    public abstract JsonSchema? GetDataSchema();

    public virtual bool Matches(Decision decision)
        => decision.RawResponse?.PickedAction == Name;
}

public abstract class ActionBase<T>(string name, string description = "") : ActionBase(name, description)
{
    public abstract bool TryDeserialize(JToken? data, [NotNullWhen(true)] out T? response);

    public override bool Matches(Decision decision) => Matches(decision, out _);

    public bool Matches(Decision decision, [NotNullWhen(true)] out T? response)
    {
        response = default;
        return base.Matches(decision) && TryDeserialize(decision.RawResponse?.Data, out response);
    }
}
