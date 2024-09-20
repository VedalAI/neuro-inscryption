using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class PlaceCardAction(CombatState state) : IAction<int>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionIds.ChooseSlotForCard),
                ["data"] = QJS.Enum(state.PlayerLanes.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).Select(int.Parse))
            }
        });
    }

    string IAction.ActionName => ActionIds.ChooseSlotForCard;
    bool IAction<int>.Deserialize(Action action, out int response)
    {
        response = action.ActionData?.Value<int>() ?? -1;
        return response != -1;
    }
}
