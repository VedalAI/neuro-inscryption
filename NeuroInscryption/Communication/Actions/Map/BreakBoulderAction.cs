using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class BreakBoulderAction() : ActionBase<int>(ActionStrings.BreakBoulder)
{
    public override JsonSchema GetDataSchema()
    {
        return new()
        {
            Type = JsonSchemaType.Integer,
            Minimum = 1,
            Maximum = 3
        };
    }

    public override bool TryDeserialize(JToken? data, out int response)
    {
        if (!data.TryValue(out int? boulder) || boulder is null or < 1 or > 3)
        {
            response = -1;
            return false;
        }

        response = boulder.Value;
        return true;
    }
}
