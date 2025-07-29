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
                    .Select(pair => pair.index + CombatState.LaneIndexBase))
            },
            Required = ["lane"]
        };
    }

    public override bool TryDeserialize(JToken? data, out int response)
    {
        int? result = data?.Value<int>("lane");
        if (!result.HasValue)
        {
            response = 0;
            return false;
        }

        int actualLane = result.Value - CombatState.LaneIndexBase;
        response = actualLane;
        return playerLanes.WithIndex().Where(pair => pair.item.Card == null).Any(pair => pair.index == actualLane);
    }
}
