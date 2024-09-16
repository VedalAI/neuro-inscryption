using System.Collections.Generic;
using NeuroInscryption.Communication.Json;

namespace NeuroInscryption.Communication.Actions;

public sealed class EndTurnAction : IAction
{
    private const string _actionName = "end_turn";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(_actionName),
                ["data"] = QJS.ConstNull
            }
        });
    }

    string IAction.ActionName => _actionName;
}
