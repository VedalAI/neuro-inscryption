using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using Newtonsoft.Json.Linq;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Communication.Actions;

public sealed class ChooseCardAction(GameState.Board state) : IAction<string>
{
    private const string _actionName = "choose_card_to_play";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (PlayerHand.Instance.CardsInHand.Count == 0) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(_actionName),
                ["data"] = QJS.Enum(state.PlayerHand.Where(c => c.IsPlayable == true).Select(c => c.Name))
            }
        });
    }

    string IAction.ActionName => _actionName;

    bool IAction<string>.Deserialize(Action action, out string? response)
    {
        response = action.ActionData?.Value<string>();
        return response != null;
    }
}
