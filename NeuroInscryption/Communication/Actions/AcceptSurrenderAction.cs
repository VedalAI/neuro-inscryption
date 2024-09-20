using System.Collections.Generic;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;

namespace NeuroInscryption.Communication.Actions;

public sealed class AcceptSurrenderAction(CombatState state) : IAction
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
