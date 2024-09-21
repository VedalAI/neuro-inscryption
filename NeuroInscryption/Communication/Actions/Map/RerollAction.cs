using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class RerollAction(MainInputInteractable clover) : IAction
{
    public bool Applicable { get; } = clover.Enabled && clover.gameObject.activeSelf;

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (!Applicable) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionStrings.Reroll),
                ["data"] = QJS.ConstNull,
            }
        });
    }

    string IAction.ActionName => ActionStrings.Reroll;
}
