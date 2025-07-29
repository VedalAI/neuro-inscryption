using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions;

public sealed class ChooseMultipleAction<TSelectable>(IEnumerable<TSelectable> items, int amount)
    : ActionBase<IEnumerable<TSelectable>>(ActionStrings.MakeChoice)
    where TSelectable : IIdentifiable
{
    public override bool IsApplicable => amount > 0 && items.Any();
    public override JsonSchema GetDataSchema()
    {
        return new()
        {
            Type = JsonSchemaType.Array,
            MinItems = amount,
            MaxItems = amount,
            UniqueItems = true,
            Items = QJS.Enum(items.Select(c => c.Identifier)),
        };
    }

    public override bool TryDeserialize(JToken? data, [NotNullWhen(true)] out IEnumerable<TSelectable>? response)
    {
        IEnumerable<string?>? cardIdentifiers = data?.Values<string>()!;
        if (cardIdentifiers == null)
        {
            response = null;
            return false;
        }

        response = cardIdentifiers.Select(id => items.First(c => c?.Identifier == id));
        return true;
    }
}
