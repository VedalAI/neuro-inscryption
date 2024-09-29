using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class ChooseTraderCardsAction(IEnumerable<SelectableCardData> cards, int amount) : IAction<IEnumerable<SelectableCardData>>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionName),
                ["data"] = new()
                {
                    Type = JsonSchemaType.Array,
                    MinItems = amount,
                    MaxItems = amount,
                    UniqueItems = true,
                    Items = QJS.Enum(cards.Select(c => c.Identifier)),
                }
            }
        });
    }

    public string ActionName => ActionStrings.MakeChoice;

    bool IAction<IEnumerable<SelectableCardData>>.Deserialize(Action action, out IEnumerable<SelectableCardData>? response)
    {
        IEnumerable<string?>? cardIdentifiers = action.ActionData?.Values<string>()!;
        if (cardIdentifiers == null)
        {
            response = null;
            return false;
        }

        response = cardIdentifiers.Select(id => cards.First(c => c?.Identifier == id));
        return true;
    }
}
