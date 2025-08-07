using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;

namespace NeuroInscryption.Communication.Actions.Map;

public sealed class BreakBoulderAction(IEnumerable<SelectableCard> boulders, CancellationToken ct = default) : AsyncNeuroAction<SelectableCard>(ct)
{
    public override string Name => ActionStrings.BreakBoulder;
    
    protected override JsonSchema Schema => QJS.WrapObject(new Dictionary<string, JsonSchema>
    {
        ["boulder"] = QJS.Enum([1, 2, 3])
    });
    
    protected override ExecutionResult Validate(ActionJData actionData, out SelectableCard? boulder)
    {
        int index = actionData.GetValue<int>("boulder");
        if (index is < 1 or > 3) index = 2;
        
        boulder = boulders.ElementAt(index - 1);
        
        return ExecutionResult.Success();
    }

    protected override async UniTask ExecuteAsync(SelectableCard? boulder, CancellationToken ct)
    {
        await NeuroMouse.MoveAndClick(boulder!, ct);
    }
}
