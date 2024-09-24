using System.Collections.Generic;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class AcceptSurrenderAction(CombatState state) : IAction
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (state.IsOpponentSurrendering != true) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionName),
                ["data"] = QJS.ConstNull
            }
        });
    }

    public string ActionName => ActionStrings.AcceptSurrender;
}
