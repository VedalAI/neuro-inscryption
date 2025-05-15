using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class CampfireSecondUpgradeAction() : ActionBase<bool>(ActionStrings.CampfireSecondUpgrade)
{
    public override bool TryDeserialize(JToken? data, out bool response)
    {
        return data.TryValue(out response);
    }

    public override JsonSchema GetDataSchema() => new JsonSchema { Type = JsonSchemaType.Boolean };
}
