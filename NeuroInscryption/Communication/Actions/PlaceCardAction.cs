using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using Newtonsoft.Json.Linq;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Communication.Actions;

public sealed class PlaceCardAction(GameState.Board state) : IAction<int>
{
    private const string _actionName = "choose_slot_for_card";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(_actionName),
                ["data"] = QJS.Enum(state.PlayerLanes.Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).Select(int.Parse))
            }
        });
    }

    string IAction.ActionName => _actionName;
    bool IAction<int>.Deserialize(Action action, out int response)
    {
        response = action.ActionData?.Value<int>() ?? -1;
        return response != -1;
    }
}
