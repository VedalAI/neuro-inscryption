using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class LeaveAction : IAction
{
    public void Apply(JsonSchema schema)
    {
        Dictionary<string, JsonSchema> properties = new()
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.ConstNull
        };

        schema.Add(properties);
    }

    public string ActionName => ActionStrings.Leave;
}
