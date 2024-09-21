using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class PlayCardAction(CombatState state) : IAction<PlayableCardData>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (PlayerHand.Instance.CardsInHand.Count == 0) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionStrings.PlayCard),
                ["data"] = QJS.Enum(state.PlayerHand.Where(c => c.IsPlayable == true).Select(c => c.Identifier))
            }
        });
    }

    string IAction.ActionName => ActionStrings.PlayCard;

    bool IAction<PlayableCardData>.Deserialize(Action action, out PlayableCardData? response)
    {
        string? cardIdentifier = action.ActionData?.Value<string>();
        if (cardIdentifier == null)
        {
            response = null;
            return false;
        }

        response = state.PlayerHand.First(c => c.Identifier == cardIdentifier);
        return true;
    }
}
