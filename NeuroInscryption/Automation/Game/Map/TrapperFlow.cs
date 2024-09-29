using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
using Sirenix.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class TrapperFlow : HarmonyAutomationFlow<object>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(int teeth, IEnumerable<SelectableCard> pelts, IEnumerable<int> peltPrices, bool knifeAvailable, MainInputInteractable knife, int knifePrice, IGameState state) : ILoadoutContext
    {
        public readonly IReadOnlyList<TrapperWareData> ItemsForSale = pelts.Zip(peltPrices, (c, p) => TrapperWareData.CreatePelt(c, p, state)).AppendIf(knifeAvailable, TrapperWareData.CreateKnife(knife, knifePrice)).Where(w => w.PriceNum <= teeth).ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuyPeltsSequencer), nameof(BuyPeltsSequencer.BuyPelts))),
            IsValid = context => context.GetInstance()!.GetEnumeratorState() == 21,
        }
    ];

    protected override async UniTask ExecuteAsync(object target, CancellationToken ct)
    {
        await UniTask.Delay(200, cancellationToken: ct);
        await RunFlowAsync(target.GetEnumeratorThis<BuyPeltsSequencer>(), ct);
    }

    private async UniTask RunFlowAsync(BuyPeltsSequencer instance, CancellationToken ct)
    {
        await UniTask.Delay(300, cancellationToken: ct);

        LoadoutState state = LoadoutState.Calculate();
        Context context = new(state.Teeth, instance.peltsForSale, instance.PeltPrices, !RunState.Run.trapperKnifeBought && RunState.Run.consumables.Count < RunState.Run.MaxConsumables, instance.purchaseKnifeInteractable, 7, state);
        state.SetContext(context);

        ChooseAction<TrapperWareData> buyAction = new(context.ItemsForSale, false, ActionStrings.BuyTrapperItem);
        LeaveAction leaveAction = new(!buyAction.Applicable);

        Decision decision = new(DecisionStrings.BuyTrapperItems.FormatPlural(state.Teeth), state);
        decision.AddAction(buyAction);
        decision.AddAction(leaveAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(buyAction, out TrapperWareData? trapperWare))
        {
            if (trapperWare.IsKnife)
            {
                if (ViewManager.Instance.CurrentView != View.Default)
                {
                    ViewManager.Instance.SwitchToView(View.Default);
                    await UniTask.Delay(500, cancellationToken: ct);
                }

                await NeuroMouse.MoveToAsync(trapperWare.Interactable, 200, ct);
                await NeuroMouse.ClickAsync(ct);
            }
            else
            {
                if (ViewManager.Instance.CurrentView != View.TradingTopDown)
                {
                    ViewManager.Instance.SwitchToView(View.TradingTopDown);
                    await UniTask.Delay(500, cancellationToken: ct);
                }

                await NeuroMouse.MoveToAsync(trapperWare.Interactable, 200, ct);
                await NeuroMouse.ClickAsync(ct);
            }

            await RunFlowAsync(instance, ct);
        }
        else if (action.Match(leaveAction))
        {
            await NeuroMouse.MoveToAsync(instance.purchasedPile, 200, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }
}
