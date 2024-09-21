using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class ChooseCardAction(IEnumerable<SelectableCardData> cards, bool isOnlyOption) : IAction<SelectableCardData>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        Dictionary<string, JsonSchema> properties = new()
        {
            ["action"] = QJS.Const(ActionStrings.ChooseCard),
            ["data"] = QJS.Enum(cards.Select(card => card.Identifier))
        };

        if (isOnlyOption) schema.Properties = properties;
        else schema.OneOf.Add(new JsonSchema { Properties = properties });
    }

    string IAction.ActionName => ActionStrings.ChooseCard;

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
