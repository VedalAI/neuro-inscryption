using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class BuildTotemAction(IEnumerable<TotemPieceData> pieces) : ActionBase<BuildTotemAction.Response>(ActionStrings.BuildTotem)
{
    public readonly record struct Response(TotemPieceData Top, TotemPieceData Bottom);

    public override JsonSchema GetDataSchema()
    {
        return new()
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["tribe"] = QJS.Enum(pieces.Select(p => p.Tribe).WhereNotNull()),
                ["sigil"] = QJS.Enum(pieces.Select(p => p.SigilName).WhereNotNull())
            },
            Required = ["tribe", "sigil"],
        };
    }

    public override bool TryDeserialize(JToken? data, out Response response)
    {
        string? tribe = data?.Value<string>("tribe");
        string? sigil = data?.Value<string>("sigil");
        if (tribe == null || sigil == null)
        {
            response = default;
            return false;
        }

        TotemPieceData top = pieces.Single(p => p.Tribe == tribe);
        TotemPieceData bottom = pieces.Single(p => p.SigilName == sigil);

        response = new Response(top, bottom);
        return true;
    }
}
