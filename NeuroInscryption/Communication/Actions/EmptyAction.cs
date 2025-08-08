using NeuroSdk.Json;

namespace NeuroInscryption.Communication.Actions;

/// <summary>Generic choosable action with no data.</summary>
/// <param name="actionName">The unique identifier for this action, to distinguish it from other potential actions.</param>
public class EmptyAction(string actionName, string description = "") : ActionBase(actionName, description)
{
    public override JsonSchema GetDataSchema() => QJS.ConstNull;
}
