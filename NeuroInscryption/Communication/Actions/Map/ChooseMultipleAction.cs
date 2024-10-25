using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;
using Action = NeuroInscryption.Communication.Messages.Incoming.Action;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class ChooseMultipleAction<TSelectable>(IEnumerable<TSelectable> cards, int amount) : IAction<IEnumerable<TSelectable>>
    where TSelectable : IIdentifiable
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.Add(new Dictionary<string, JsonSchema>
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
        });
    }

    public string ActionName => ActionStrings.MakeChoice;

    bool IAction<IEnumerable<TSelectable>>.Deserialize(Action action, out IEnumerable<TSelectable>? response)
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
