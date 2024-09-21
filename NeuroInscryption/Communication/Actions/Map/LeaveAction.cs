using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class LeaveAction(bool isOnlyOption) : IAction
{
    public void Apply(JsonSchema schema)
    {
        Dictionary<string, JsonSchema> properties = new()
        {
            ["action"] = QJS.Const(ActionStrings.Leave),
            ["data"] = QJS.ConstNull
        };

        if (isOnlyOption) schema.Properties = properties;
        else schema.OneOf.Add(new JsonSchema { Properties = properties });
    }

    public string ActionName => ActionStrings.Leave;
}
