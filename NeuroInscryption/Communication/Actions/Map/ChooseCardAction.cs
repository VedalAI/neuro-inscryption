using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public class ChooseCardAction(IEnumerable<SelectableCardData> cards) : IAction<SelectableCardData>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionIds.ChooseCard),
                ["data"] = QJS.Enum(cards.Select(card => card.Identifier))
            }
        });
    }

    string IAction.ActionName => ActionIds.ChooseCard;

    bool IAction<SelectableCardData>.Deserialize(Action action, out SelectableCardData? response)
    {
        string? identifier = action.ActionData?.Value<string>();
        if (identifier == null)
        {
            response = null;
            return false;
        }

        response = cards.FirstOrDefault(node => node.Identifier == identifier);
        return response != null;
    }
}
