using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class AcceptSurrenderAction(bool? isOpponentSurrendering) : EmptyAsyncNeuroAction
{
    public override string Name => ActionStrings.AcceptSurrender;
    public override bool IsApplicable => isOpponentSurrendering == true;

    protected override async UniTask ExecuteAsync(CancellationToken ct)
    {
        await UniTask.Delay(500, cancellationToken: ct); // branch takes time to extend, so the mouse aim gets thrown off
        await NeuroMouse.MoveAndClick(LeshyAnimationController.Instance.oliveBranchInteractable, ct, msDelayBeforeClick: 500);
    }
}
