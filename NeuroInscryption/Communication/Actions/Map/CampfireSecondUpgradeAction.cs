using System.Collections.Generic;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class CampfireSecondUpgradeAction : IAction
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionName),
                ["data"] = QJS.ConstNull,
            }
        });
    }

    public string ActionName => ActionStrings.CampfireSecondUpgrade;
}
