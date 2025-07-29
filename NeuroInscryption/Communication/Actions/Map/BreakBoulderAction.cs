using NeuroInscryption.Communication.Strings;
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
        int boulder = data?.Value<int>() ?? -1;
        if (boulder is < 1 or > 3)
        {
            response = -1;
            return false;
        }

        response = boulder;
        return true;
    }
}
