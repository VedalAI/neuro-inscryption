using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Combat;

public sealed class DrawCardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(CardDrawPiles3D), nameof(CardDrawPiles3D.ChooseDraw))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(2)
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator _, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        bool drawFromSideDeck = await Neuro.Combat.ChooseDrawFromSideDeck(ct);

        CardPile target = !drawFromSideDeck ? CardDrawPiles3D.Instance.Pile : CardDrawPiles3D.Instance.SidePile;
        await NeuroMouse.MoveAndClick(target, ct, msDelayBeforeClick: 500);
    }
}
