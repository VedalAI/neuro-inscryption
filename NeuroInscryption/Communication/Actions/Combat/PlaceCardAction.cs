using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class PlaceCardAction(IReadOnlyDictionary<string, PlayableCardData?> playerLanes) : IAction<int>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOfAdd(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(playerLanes.Where(kvp => kvp.Value == null)
                .Select(kvp => CombatState.LaneIndexBase + int.Parse(kvp.Key)))
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
