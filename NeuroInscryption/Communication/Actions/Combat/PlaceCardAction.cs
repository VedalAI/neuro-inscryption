using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class PlaceCardAction(IReadOnlyCollection<CardSlot> playerLanes)
    : ActionBase<int>(ActionStrings.PlaceCard)
{
    public override JsonSchema GetDataSchema()
    {
        return new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, JsonSchema>
            {
                ["lane"] = QJS.Enum(playerLanes.WithIndex()
                    .Where(pair => pair.item.Card == null)
                    .Select(pair => CombatState.LaneIndexBase + pair.index))
            },
            Required = ["lane"]
        };
    }

    public override bool TryDeserialize(JToken? data, out int response)
    {
        int? result = data?.Value<int>("lane");
        response = result.HasValue ? result.Value - CombatState.LaneIndexBase : 0;
        return result.HasValue;
    }
}
