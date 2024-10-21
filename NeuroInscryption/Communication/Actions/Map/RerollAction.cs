using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class RerollAction(MainInputInteractable clover) : IAction
{
    public bool Applicable { get; } = clover && clover.Enabled && clover.gameObject.activeSelf;

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        if (!Applicable) return;

        schema.Add(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.ConstNull,
        });
    }

    public string ActionName => ActionStrings.Reroll;
}
