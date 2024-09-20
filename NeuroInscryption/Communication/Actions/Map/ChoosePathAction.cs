using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class ChoosePathAction(IEnumerable<string> nextNodes) : IAction<string>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        // this is only action, so we dont have OneOf
        schema.Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionIds.ChoosePath),
            ["data"] = QJS.Enum(nextNodes)
        };
    }

    string IAction.ActionName => ActionIds.ChoosePath;

    bool IAction<string>.Deserialize(Action action, out string? response)
    {
        response = action.ActionData?.Value<string>();
        return response != null;
    }
}
