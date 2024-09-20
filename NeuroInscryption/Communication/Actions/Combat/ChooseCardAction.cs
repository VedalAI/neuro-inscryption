using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class ChooseCardAction(CombatState state) : IAction<string>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (PlayerHand.Instance.CardsInHand.Count == 0) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionIds.ChooseCardToPlay),
                ["data"] = QJS.Enum(state.PlayerHand.Where(c => c.IsPlayable == true).Select(c => c.Identifier))
            }
        });
    }

    string IAction.ActionName => ActionIds.ChooseCardToPlay;

    bool IAction<string>.Deserialize(Action action, out string? response)
    {
        response = action.ActionData?.Value<string>();
        return response != null;
    }
}
