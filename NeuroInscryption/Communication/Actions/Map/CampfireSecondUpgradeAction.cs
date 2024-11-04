using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class CampfireSecondUpgradeAction : IAction<bool>
{
    public string ActionName => ActionStrings.CampfireSecondUpgrade;

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = new() { Type = JsonSchemaType.Boolean },
        };
    }

    public bool Deserialize(Action action, out bool response)
    {
        bool? value = action.ActionData?.Value<bool>();
        response = value ?? false;
        return value.HasValue;
    }
}
