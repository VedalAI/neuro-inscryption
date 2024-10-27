using System.Collections.Generic;
using NeuroInscryption.Communication.Json;

namespace NeuroInscryption.Communication.Actions;

/// <summary>Generic choosable action with no data.</summary>
/// <param name="actionName">The unique identifier for this action, to distinguish it from other potential actions.</param>
/// <param name="isApplicable">If <see langword="false"/>, will not be added to the response schema.</param>
public class BasicAction(string actionName) : IAction
{
    protected virtual bool IsApplicable => true;

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (!IsApplicable) return;

        schema.OneOfAdd(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.ConstNull,
        });
    }

    public virtual string ActionName { get; } = actionName;
}