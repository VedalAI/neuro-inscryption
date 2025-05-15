using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Actions;

public sealed class ChooseAction<T>(IEnumerable<T> items, string name = ActionStrings.MakeChoice) : ActionBase<T>(name)
    where T : class, IIdentifiable
{
    public override bool IsApplicable => items.Any();

    public override bool TryDeserialize(JToken? data, [NotNullWhen(true)] out T? response)
    {
        if (!data.TryValue(out string? identifier) || identifier is null)
        {
            response = null;
            return false;
        }

        response = items.FirstOrDefault(node => node.Identifier == identifier);
        return response != null;
    }

    public override JsonSchema GetDataSchema() => QJS.Enum(items.Select(item => item.Identifier).Distinct());
}
