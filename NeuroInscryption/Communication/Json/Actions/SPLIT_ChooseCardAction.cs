using System.Collections.Generic;
using System.Linq;
using DiskCardGame;

namespace NeuroInscryption.Communication.Json.Actions;

// ReSharper disable once InconsistentNaming
internal sealed class SPLIT_ChooseCardAction : ISchemaModifier
{
    public void Apply(JsonSchema schema)
    {
        if (PlayerHand.Instance.CardsInHand.Count == 0) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const("choose_card_to_play"),
                ["data"] = QJS.Enum(GameState.Board.Current.PlayerHand.Where(c => c.IsPlayable == true).Select(c => c.Name))
            }
        });
    }
}
