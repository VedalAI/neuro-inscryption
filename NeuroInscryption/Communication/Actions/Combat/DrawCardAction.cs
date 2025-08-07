using System.Collections.Generic;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class DrawCardAction(bool mainDeckEmpty, bool sideDeckEmpty)
    : ActionBase<bool>(ActionStrings.DrawCard)
{
    public override JsonSchema GetDataSchema()
    {
        List<string> decks = [];
        if (!mainDeckEmpty) decks.Add("Random");
        if (!sideDeckEmpty) decks.Add("Squirrel");

        return new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, JsonSchema>
            {
                ["deck"] = QJS.Enum(decks),
            },
            Required = ["deck"]
        };
    }

    public override bool TryDeserialize(JToken? data, out bool drawFromSideDeck)
    {
        string? strResponse = data?.Value<string>("deck");

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
