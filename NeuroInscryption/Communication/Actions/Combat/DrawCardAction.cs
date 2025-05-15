using System.Collections.Generic;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class DrawCardAction(bool mainDeckEmpty, bool sideDeckEmpty)
    : ActionBase<bool>(ActionStrings.DrawCard)
{
    public override JsonSchema GetDataSchema()
    {
        List<string> values = [];
        if (!mainDeckEmpty) values.Add("Random");
        if (!sideDeckEmpty) values.Add("Squirrel");
        return QJS.Enum(values);
    }

    public override bool TryDeserialize(JToken? data, out bool drawFromSideDeck)
    {
        if (!data.TryValue(out string? strResponse))
        {
            drawFromSideDeck = false;
            return false;
        }

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
