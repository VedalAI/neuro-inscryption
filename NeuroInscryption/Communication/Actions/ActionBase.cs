using System;
using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Automation.API;
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

    public virtual bool Validate(Decision decision)
        => decision.RawResponse?.PickedAction == Name;
}

public abstract class ActionBase<T>(string name, string description = "") : ActionBase(name, description)
{
    protected abstract bool TryDeserialize(JToken? data, [NotNullWhen(true)] out T? response);
    private bool _hasResult; // used to just be a null check on _result but would break if T is a nullable reference type
    private T? _result;
    /// <summary>
    /// Call this <b>only</b> after adding this action to a decision and awaiting it.
    /// </summary>
    public T GetResult()
    {
        AutomationManager.Assert(_hasResult, $"Tried to get result for action {Name} before result was set");
        return _result!;
    }

    public override bool Validate(Decision decision)
    {
        bool valid = base.Validate(decision) && TryDeserialize(decision.RawResponse?.Data, out _result);
        if (valid)
        {
            if (_hasResult)
                throw new InvalidOperationException($"Do not reuse action objects - tried to set result on action {Name} but it's already been validated before");
            _hasResult = true;
        }
        return valid;
    }
}
