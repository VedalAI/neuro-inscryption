using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class BuildTotemAction(IEnumerable<TotemPieceData> pieces) : IAction<BuildTotemAction.Response>
{
    public readonly record struct Response(TotemPieceData Top, TotemPieceData Bottom);

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = new()
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["tribe"] = QJS.Enum(pieces.Select(p => p.Tribe).WhereNotNull()),
                    ["sigil"] = QJS.Enum(pieces.Select(p => p.SigilName).WhereNotNull())
                },
                Required = ["tribe", "sigil"],
            }
        };
    }

    public string ActionName => ActionStrings.BuildTotem;

    bool IAction<Response>.Deserialize(Action action, out Response response)
    {
        string? tribe = action.ActionData?.Value<string>("tribe");
        string? sigil = action.ActionData?.Value<string>("sigil");
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
