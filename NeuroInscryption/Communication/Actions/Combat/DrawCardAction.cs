using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class DrawCardAction(bool mainDeckEmpty, bool sideDeckEmpty) : IAction<bool>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        List<string> values = [];
        if (!mainDeckEmpty) values.Add("Random");
        if (!sideDeckEmpty) values.Add("Squirrel");
        schema.OneOfAdd(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(values)
        });
    }

    public string ActionName => ActionStrings.DrawCard;

    bool IAction<bool>.Deserialize(Action action, out bool drawFromSideDeck)
    {
        string? strResponse = action.ActionData?.Value<string>();

        switch (strResponse)
        {
            case "Random":
                drawFromSideDeck = mainDeckEmpty;
                return true;

            case "Squirrel":
                drawFromSideDeck = !sideDeckEmpty;
                return true;

            default:
                drawFromSideDeck = false;
                return false;
        }
    }
}
