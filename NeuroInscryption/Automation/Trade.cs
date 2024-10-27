using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using Sirenix.Utilities;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal class Trade
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class TrapperShopContext(int teeth, IEnumerable<SelectableCard> pelts, IEnumerable<int> peltPrices, bool knifeAvailable, MainInputInteractable knife, int knifePrice) : ILoadoutContext
    {
        public readonly IReadOnlyList<TrapperWareData> ItemsForSale = pelts
            .Zip(peltPrices, TrapperWareData.CreatePelt)
            .AppendIf(knifeAvailable, TrapperWareData.CreateKnife(knife, knifePrice))
            .Where(w => w.PriceNum <= teeth)
            .ToList();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    // no IAsyncEnumerable grrrrrr
    public UniTaskCancelableAsyncEnumerable<TrapperWareData> BuyFromTrapper(BuyPeltsSequencer instance, CancellationToken ct = default)
    {
        return UniTaskAsyncEnumerable.Create<TrapperWareData>(async (writer, token) =>
        {
            while (true)
            {
                if (token.IsCancellationRequested) return;

                int teeth = RunState.Run.currency;
                int cheapest = instance.PeltPrices[0]; // at most 4 and always cheaper than the other wares

                if (teeth < cheapest)
                {
                    Plugin.Logger.LogMessage($"Can't buy anything with {teeth} teeth (cheapest pelt costs {cheapest}), leaving");
                    return;
                }

                LoadoutState state = LoadoutState.Calculate();
                TrapperShopContext context = new(teeth, instance.peltsForSale, instance.PeltPrices, !RunState.Run.trapperKnifeBought && RunState.Run.consumables.Count < RunState.Run.MaxConsumables, instance.purchaseKnifeInteractable, 7);
                state.SetContext(context);

                ChooseAction<TrapperWareData> buyAction = new(context.ItemsForSale, ActionStrings.BuyTrapperItem);
                LeaveAction leaveAction = new();

                Decision decision = new(DecisionStrings.BuyTrapperItems.Format(teeth), state);
                decision.AddAction(buyAction);
                decision.AddAction(leaveAction);

                Action action = await decision.SendAsync(token);

                if (action.Match(buyAction, out TrapperWareData? trapperWare))
                {
                    await writer.YieldAsync(trapperWare);
                }
                else if (action.Match(leaveAction))
                {
                    return;
                }
                else
                {
                    action.ThrowInvalid();
                }
            }
        }).WithCancellation(ct);
    }
}
