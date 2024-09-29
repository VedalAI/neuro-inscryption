using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class PlaceCardAction(IReadOnlyDictionary<string, PlayableCardData?> playerLanes) : IAction<int>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionName),
                ["data"] = QJS.Enum(playerLanes.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).Select(int.Parse))
            }
        });
    }

    public string ActionName => ActionStrings.PlaceCard;

    bool IAction<int>.Deserialize(Action action, out int response)
    {
        response = action.ActionData?.Value<int>() ?? -1;
        return response != -1;
    }
}
