using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class EndTurnAction : EmptyAsyncNeuroAction
{
    public override string Name => ActionStrings.EndTurn;
    public override bool IsApplicable => true;
    
    protected override async UniTask ExecuteAsync(CancellationToken ct)
    {
        await NeuroMouse.MoveAndClick(((BoardManager3D)BoardManager.Instance).Bell, ct, msDelayBeforeClick: 500);
    }
}
