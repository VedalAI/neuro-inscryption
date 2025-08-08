using System.Collections.Generic;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class CampfireSecondUpgradeAction() : ActionBase<bool>(ActionStrings.CampfireSecondUpgrade)
{
    protected override bool TryDeserialize(JToken? data, out bool response)
    {
        bool? value = data?.Value<bool>("wantSecondUpgrade");
        response = value ?? false;
        return value.HasValue;
    }

    public override JsonSchema GetDataSchema()
    {
        return new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, JsonSchema>
            {
                ["wantSecondUpgrade"] = QJS.Type(JsonSchemaType.Boolean)
            },
            Required = ["wantSecondUpgrade"],
        };
    }
}
