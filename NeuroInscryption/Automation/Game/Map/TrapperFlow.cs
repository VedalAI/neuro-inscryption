using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class TrapperFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuyPeltsSequencer), nameof(BuyPeltsSequencer.BuyPelts))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(21),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        await Wait.CommonFlowDelay(ct);

        BuyPeltsSequencer instance = target.GetThis<BuyPeltsSequencer>();
        await foreach (TrapperWareData trapperWare in Neuro.Trade.BuyFromTrapper(instance, ct))
        {
            if (ViewManager.Instance.CurrentView != trapperWare.TargetView)
            {
                ViewManager.Instance.SwitchToView(trapperWare.TargetView);
                await Wait.CommonFlowDelay(ct);
            }

            await NeuroMouse.MoveAndClick(trapperWare.Interactable, ct);
        }

        await NeuroMouse.MoveAndClick(instance.purchasedPile, ct);
    }
}
