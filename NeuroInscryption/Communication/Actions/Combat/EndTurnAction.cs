using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class EndTurnAction : IAction
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionIds.EndTurn),
                ["data"] = QJS.ConstNull
            }
        });
    }

    string IAction.ActionName => ActionIds.EndTurn;
}
