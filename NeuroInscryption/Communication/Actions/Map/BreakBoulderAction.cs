using System.Collections.Generic;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class BreakBoulderAction() : ActionBase<int>(ActionStrings.BreakBoulder)
{
    public override JsonSchema GetDataSchema()
    {
        return new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, JsonSchema>
            {
                ["boulder"] = QJS.Enum([1, 2, 3])
            },
            Required = ["boulder"],
        };
    }

    protected override bool TryDeserialize(JToken? data, out int response)
    {
        int boulder = data?.Value<int>("boulder") ?? -1;
        if (boulder is < 1 or > 3)
        {
            response = 2;
            return true;
        }

        response = boulder;
        return true;
    }
}
