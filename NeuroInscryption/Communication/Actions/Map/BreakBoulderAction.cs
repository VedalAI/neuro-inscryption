using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class BreakBoulderAction : IAction<int>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = new()
            {
                Type = JsonSchemaType.Integer,
                Minimum = 1,
                Maximum = 3
            }
        };
    }

    public string ActionName => ActionStrings.BreakBoulder;

    bool IAction<int>.Deserialize(Action action, out int response)
    {
        int boulder = action.ActionData?.Value<int>() ?? -1;
        if (boulder is < 1 or > 3)
        {
            response = -1;
            return false;
        }

        response = boulder;
        return true;
    }
}
