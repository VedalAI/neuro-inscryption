using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class RerollAction(MainInputInteractable clover) : IAction
{
    public void Apply(JsonSchema schema)
    {
        if (!clover.Enabled) return;

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(ActionIds.Reroll),
                ["data"] = QJS.ConstNull,
            }
        });
    }

    string IAction.ActionName => ActionIds.Reroll;
}
