using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class ChooseAction<T>(IEnumerable<T> items, bool isOnlyOption) : IAction<T>
    where T : class, IIdentifiable
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        Dictionary<string, JsonSchema> properties = new()
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(items.Select(card => card.Identifier))
        };

        if (isOnlyOption) schema.Properties = properties;
        else schema.OneOf.Add(new JsonSchema { Properties = properties });
    }

    public string ActionName => ActionStrings.MakeChoice;

    bool IAction<T>.Deserialize(Action action, out T? response)
    {
        string? identifier = action.ActionData?.Value<string>();
        if (identifier == null)
        {
            response = null;
            return false;
        }

        response = items.FirstOrDefault(node => node.Identifier == identifier);
        return response != null;
    }
}
