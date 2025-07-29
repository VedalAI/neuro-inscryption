using NeuroInscryption.Communication.Strings;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class CampfireSecondUpgradeAction() : ActionBase<bool>(ActionStrings.CampfireSecondUpgrade)
{
    public override bool TryDeserialize(JToken? data, out bool response)
    {
        bool? value = data?.Value<bool>();
        response = value ?? false;
        return value.HasValue;
    }

    public override JsonSchema GetDataSchema() => new JsonSchema { Type = JsonSchemaType.Boolean };
}
