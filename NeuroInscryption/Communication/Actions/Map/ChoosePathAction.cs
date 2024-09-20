using System.Collections.Generic;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class ChoosePathAction(IEnumerable<MapNodeData> nextNodes) : IAction<MapNodeData>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        // this is only action, so we dont have OneOf
        schema.Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionIds.ChoosePath),
            ["data"] = QJS.Enum(nextNodes.Select(node => node.NameWithDiscriminator))
        };
    }

    string IAction.ActionName => ActionIds.ChoosePath;

    bool IAction<MapNodeData>.Deserialize(Action action, out MapNodeData? response)
    {
        string? nodeIdentifier = action.ActionData?.Value<string>();
        if (nodeIdentifier == null)
        {
            response = null;
            return false;
        }

        response = nextNodes.FirstOrDefault(node => node.NameWithDiscriminator == nodeIdentifier);
        return response != null;
    }
}
