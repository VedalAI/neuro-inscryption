using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class DrawCardAction(bool mainDeckEmpty, bool sideDeckEmpty)
    : AsyncNeuroAction<bool>
{
    public override string Name => ActionStrings.DrawCard;
    
    protected override JsonSchema Schema => new()
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, JsonSchema>
        {
            ["deck"] = QJS.Enum(Enumerable.Empty<string>().Concat(!mainDeckEmpty ? ["Main Deck"] : []).Concat(!sideDeckEmpty ? ["Squirrel"] : [])),
        },
        Required = ["deck"]
    };

    public override bool IsApplicable => true;
    
    protected override ExecutionResult Validate(ActionJData actionData, out bool drawFromSideDeck)
    {
        string? strResponse = actionData.GetValue<string>("deck");

        switch (strResponse)
        {
            case "Main Deck":
            {
                if (mainDeckEmpty)
                {
                    drawFromSideDeck = false;
                    return ExecutionResult.Failure("Main deck is empty, cannot draw from it.");
                }
                drawFromSideDeck = false;
                return ExecutionResult.Success();
            }

            case "Squirrel":
            {
                if (sideDeckEmpty)
                {
                    drawFromSideDeck = false;
                    return ExecutionResult.Failure("Side deck is empty, cannot draw from it.");
                }
                drawFromSideDeck = true;
                return ExecutionResult.Success();
            }

            default:
            {
                drawFromSideDeck = false;
                return ExecutionResult.Failure($"Invalid deck choice: {strResponse}. Expected 'Main Deck' or 'Squirrel'.");
            }
        }
    }

    protected override async UniTask ExecuteAsync(bool drawFromSideDeck, CancellationToken ct)
    {
        CardPile target = !drawFromSideDeck ? CardDrawPiles3D.Instance.Pile : CardDrawPiles3D.Instance.SidePile;
        await NeuroMouse.MoveAndClick(target, ct, msDelayBeforeClick: 500);
    }
}
