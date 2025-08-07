using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;

namespace NeuroInscryption.Communication.Actions;

public class ChooseAction<T>(IEnumerable<T> items, Func<T, CancellationToken, UniTask> handler, string name = ActionStrings.MakeChoice) : AsyncNeuroAction<T>
    where T : class, IIdentifiable
{
    public override string Name => name;

    public override bool IsApplicable => items.Any();

    protected override JsonSchema Schema => new()
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, JsonSchema>
        {
            ["choice"] = QJS.Enum(items.Select(item => item.Identifier).Distinct()),
        },
        Required = ["choice"],
    };
    
    protected override ExecutionResult Validate(ActionJData actionData, out T? response)
    {
        string? identifier = actionData.GetValue<string>("choice");
        if (identifier == null)
        {
            response = null;
            return ExecutionResult.Failure("Choice is missing.");
        }
        
        response = items.FirstOrDefault(item => item.Identifier == identifier);
        return response != null ? ExecutionResult.Success() : ExecutionResult.Failure($"Choice '{identifier}' is not valid.");
    }

    protected override UniTask ExecuteAsync(T? parsedData, CancellationToken ct)
    {
        return handler(parsedData!, ct);
    }
}
