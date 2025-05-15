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
        return QJS.Enum(playerLanes.WithIndex()
            .Where(pair => pair.item.Card == null)
            .Select(pair => CombatState.LaneIndexBase + pair.index));
    }

    public override bool TryDeserialize(JToken? data, out int response)
    {
        if (!data.TryValue(out int? resp) || !resp.HasValue)
        {
            response = default;
            return false;
        }
        response = resp.Value - CombatState.LaneIndexBase;
        return true;
    }
}
