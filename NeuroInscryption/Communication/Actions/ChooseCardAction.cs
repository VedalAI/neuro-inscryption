using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Json;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Communication.Actions;

public sealed class ChooseCardAction(GameState.Board state) : ISchemaModifier
{
    public void Apply(JsonSchema schema)
    {
        if (PlayerHand.Instance.CardsInHand.Count == 0) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const("choose_card_to_play"),
                ["data"] = QJS.Enum(state.PlayerHand.Where(c => c.IsPlayable == true).Select(c => c.Name))
            }
        });
    }
}
