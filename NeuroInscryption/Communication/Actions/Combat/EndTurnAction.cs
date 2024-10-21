using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class EndTurnAction : IAction
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.Add(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.ConstNull
        });
    }

    public string ActionName => ActionStrings.EndTurn;
}
