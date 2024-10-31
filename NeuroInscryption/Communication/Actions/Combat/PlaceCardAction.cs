using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class PlaceCardAction(IReadOnlyCollection<CardSlot> playerLanes) : IAction<int>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOfAdd(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(playerLanes.WithIndex()
                .Where(pair => pair.item.Card == null)
                .Select(pair => CombatState.LaneIndexBase + pair.index))
        });
    }

    public string ActionName => ActionStrings.PlaceCard;

    bool IAction<int>.Deserialize(Action action, out int response)
    {
        int? result = action.ActionData?.Value<int>();
        response = result.HasValue ? result.Value - CombatState.LaneIndexBase : default;
        return result.HasValue;
    }
}
