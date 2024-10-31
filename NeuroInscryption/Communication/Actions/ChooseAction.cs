using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions;

public sealed class ChooseAction<T>(IEnumerable<T> items, string actionName = ActionStrings.MakeChoice) : IAction<T>
    where T : class, IIdentifiable
{
    public bool Applicable { get; } = items.Any();

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (!Applicable) return;

        Dictionary<string, JsonSchema> properties = new()
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(items.Select(item => item.Identifier).Distinct())
        };

        schema.OneOfAdd(properties);
    }

    public string ActionName => actionName;

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
