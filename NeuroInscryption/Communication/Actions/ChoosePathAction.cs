/*
using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;

namespace NeuroInscryption.Communication.Actions;

public sealed class ChoosePathAction : IAction<string>
{
    private const string _actionName = "choose_path";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        // this is only action, so we dont have OneOf
        schema.Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(_actionName),
            ["data"] = QJS.Enum() // todo: get paths from state
        };
    }

    string IAction.ActionName => _actionName;

    bool IAction<string>.Deserialize(Action action, out string? response)
    {

    }
}
*/
