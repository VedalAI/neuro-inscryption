using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions;

public sealed class DrawCardAction : IAction<bool>
{
    private const string _actionName = "draw_card";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(_actionName),
                ["data"] = QJS.Enum(["Random", "Squirrel"])
            }
        });
    }

    string IAction.ActionName => _actionName;

    bool IAction<bool>.Deserialize(Action action, out bool drawFromSideDeck)
    {
        string? strResponse = action.ActionData?.Value<string>();

        switch (strResponse)
        {
            case "Random":
                drawFromSideDeck = false;
                return true;

            case "Squirrel":
                drawFromSideDeck = true;
                return true;

            default:
                drawFromSideDeck = false;
                return false;
        }
    }
}
