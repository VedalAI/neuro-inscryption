using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Communication.Actions;

public sealed class AcceptSurrenderAction(GameState.Board state) : IAction
{
    private const string _actionName = "accept_surrender";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (state.IsOpponentSurrendering != true) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(_actionName),
                ["data"] = QJS.ConstNull
            }
        });
    }

    string IAction.ActionName => _actionName;
}
