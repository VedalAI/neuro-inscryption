using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class AcceptSurrenderAction(bool? isOpponentSurrendering) : IAction
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (isOpponentSurrendering != true) return;

        schema.Add(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.ConstNull
        });
    }

    public string ActionName => ActionStrings.AcceptSurrender;
}
