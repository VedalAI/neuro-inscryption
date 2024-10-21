using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class DrawCardAction : IAction<bool>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.Add(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(["Random", "Squirrel"])
        });
    }

    public string ActionName => ActionStrings.DrawCard;

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
